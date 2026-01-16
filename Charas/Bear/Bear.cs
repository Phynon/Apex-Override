using ApexOverride.Common;
using ApexOverride.Interfaces;
using ApexOverride.UI;
using Godot;

namespace ApexOverride.Charas.Bear;

public partial class Bear : MobBase
{
    public enum BearState
    {
        Idle,
        Walking,
        Attacking,
        Hurting,
        Dead
    }

    // animation
    private AnimationNodeStateMachinePlayback _animationStateMachine;
    private AnimationTree _animationTree;

    // hitbox
    private Area3D _attackArea;
    private CollisionShape3D _attackShape;

    // UI
    private HealthBar2D _healthBar;

    // stats
    private EntityStats _stats;
    protected override float Speed => 4.0f;
    protected override float RotateSpeed => 10.0f;

    public BearState State { get; set; }

    public BearState GetAnimationState() => State;

    public override void _Ready()
    {
        // animation
        _animationTree = GetNode<AnimationTree>("./AnimationTree");
        _animationStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
        _animationTree.Active = true;
        // hitbox
        _attackArea = GetNode<Area3D>("AttackArea");
        _attackArea.BodyEntered += OnAttackAreaEntered;
        _attackShape = _attackArea.GetNode<CollisionShape3D>("CollisionShape3D");
        _attackShape.Disabled = true;

        InitializeStats();
        InitializeUi();
    }

    private void InitializeStats()
    {
        _stats = new EntityStats();
        _stats.Name = "EntityStats";

        base.Damaged += _stats.TakeDamage;
        _stats.Died += OnDied;
        AddChild(_stats);
    }

    private void InitializeUi()
    {
        if (_stats == null) return;

        var barScene = GD.Load<PackedScene>("res://UI/HealthBar2D.tscn");
        _healthBar = barScene.Instantiate<HealthBar2D>();

        var uiLayer = GetTree().GetFirstNodeInGroup("WorldUI");
        if (uiLayer != null)
        {
            uiLayer.AddChild(_healthBar);

            // 1. FIND THE CAMERA
            // Since Bear is inside the SubViewport, this gets the correct 3D camera.

            if (GetViewport().GetCamera3D() is not Camera3d mainCam)
            {
                GD.PrintErr("Bear: No active Camera3d found in SubViewport!");
                return;
            }

            // 2. PASS IT TO THE UI
            _healthBar.Initialize(this, _stats, mainCam);
        }
    }

    // 4. ADD: Clean up when Bear is deleted
    public override void _ExitTree()
    {
        // If the bear is deleted, the UI must also be deleted manually
        // because it is NOT a child of the Bear (it's a child of CanvasLayer)
        if (IsInstanceValid(_healthBar))
        {
            _healthBar.QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _AnimationUpdate();
    }

    private void _AnimationUpdate()
    {
        if (State is BearState.Attacking or BearState.Hurting)
        {
            return;
        }

        State = Velocity.Length() > 0.05f ? BearState.Walking : BearState.Idle;
    }

    public override void Move(Vector2 targetDirection, double delta)
    {
        if (State is BearState.Attacking or BearState.Hurting)
        {
            base.Move(Vector2.Zero, delta);
            return;
        }

        base.Move(targetDirection, delta);
    }

    // attack methods
    public override void Attack()
    {
        if (State is BearState.Attacking or BearState.Hurting)
        {
            return;
        }

        State = BearState.Attacking;
        _animationStateMachine.Travel("attack");
    }

    public void OnAttackHit()
    {
        _attackShape.Disabled = false;
    }

    public void OnAttackFinished()
    {
        State = BearState.Idle;
        _attackShape.Disabled = true;
    }

    private void OnAttackAreaEntered(Node3D body)
    {
        if (body == this)
        {
            return;
        }

        IDamageable target = body as IDamageable;

        if (target == null && body.GetParent() is IDamageable parentDamageable)
        {
            target = parentDamageable;
        }

        target?.TakeDamage(10);

        // Optional: Disable hitbox immediately to prevent multi-hits
        // _attackShape.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
    }

    // hurt methods
    public override void TakeDamage(int amount)
    {
        if (State is BearState.Hurting or BearState.Dead)
        {
            return;
        }

        base.TakeDamage(amount); // This emits the Damaged signal
        GD.Print($"Bear took {amount} damage!");
        if (State is BearState.Attacking)
        {
            return;
        }

        State = BearState.Hurting;
        _animationStateMachine.Travel("flash");
    }

    public void OnHurtFinished()
    {
        State = BearState.Idle;
    }

    public void OnDied()
    {
        State = BearState.Dead;
        QueueFree();
    }
}
