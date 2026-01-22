using ApexOverride.Autoloads;
using ApexOverride.Common;
using ApexOverride.Interfaces;
using Godot;

namespace ApexOverride.Charas.Bear;

public partial class Bear : MobBase, IStatsBearer, IMeleeAttacker
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

    // stats
    private EntityStats _stats;
    protected override float Speed => 4.0f;
    protected override float RotateSpeed => 10.0f;

    public BearState State { get; set; }

    // attack methods
    public void Attack()
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

    public EntityStats GetEntityStats() => _stats;

    public void InitializeStats()
    {
        _stats = new EntityStats();
        _stats.Name = "EntityStats";
        _stats.Died += OnDied;
        AddChild(_stats);
    }

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
        UIEvents.Bus.EmitSignal(UIEvents.SignalName.HealthBarRequested, this, _stats);
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
        _stats.TakeDamage(amount);

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
