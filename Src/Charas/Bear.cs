using ApexOverride.Utils;
using Godot;

namespace ApexOverride.Charas;

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
    protected override float Speed => 4.0f;
    protected override float RotateSpeed => 10.0f;
    public BearState State { get; set; }

    public override void _Ready()
    {
        _animationTree = GetNode<AnimationTree>("./AnimationTree");
        _animationStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
        _animationTree.Active = true;
    }

    public override void Move(Vector2 direction, double delta)
    {
        if (State is BearState.Attacking or BearState.Hurting)
        {
            base.Move(Vector2.Zero, delta);
            return;
        }

        base.Move(direction, delta);
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

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _AnimationUpdate();
    }

    public void _AnimationUpdate()
    {
        if (State is BearState.Attacking or BearState.Hurting)
        {
            return;
        }

        State = Velocity.Length() > 0.01f ? BearState.Walking : BearState.Idle;
    }

    public void OnAttackFinished()
    {
        State = BearState.Idle;
    }

    public BearState GetAnimationState() => State;
}
