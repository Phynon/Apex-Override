using ApexOverride.Common;
using ApexOverride.Interfaces;
using Godot;

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

    private AnimationNodeStateMachinePlayback _animationStateMachine;

    private AnimationTree _animationTree;
    private Area3D _attackArea;
    private CollisionShape3D _attackShape;
    protected override float Speed => 4.0f;
    protected override float RotateSpeed => 10.0f;
    public BearState State { get; set; }

    public override void _Ready()
    {
        _animationTree = GetNode<AnimationTree>("./AnimationTree");
        _animationStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
        _animationTree.Active = true;

        _attackArea = GetNode<Area3D>("AttackArea");
        _attackArea.BodyEntered += OnAttackAreaEntered;
        _attackShape = _attackArea.GetNode<CollisionShape3D>("CollisionShape3D");
        _attackShape.Disabled = true;
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

        State = Velocity.Length() > 0.01f ? BearState.Walking : BearState.Idle;
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

    public BearState GetAnimationState() => State;
}
