using ApexOverride.Common;
using ApexOverride.Interfaces;
using Godot;

namespace ApexOverride.Charas.Bear;

public partial class Bear : MobBase, IStatsBearer, IMeleeAttacker
{
    // state
    public enum BearState
    {
        Idle,
        Walking,
        Attacking,
        Hurting,
        Dead
    }

    // animation tree
    private AnimationNodeStateMachinePlayback _animationStateMachine;
    private AnimationTree _animationTree;

    // combat boxes
    private Hitbox _attackHitbox;
    private Hurtbox _hurtbox;

    // stats
    private EntityStats _stats;

    public BearState State { get; set; }
    protected override float Speed => 4.0f;
    protected override float RotateSpeed => 10.0f;

    // --- chara attack ---
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
        _attackHitbox.Enable(10);
    }

    public void InitializeStats()
    {
        _stats = new EntityStats();
        _stats.Name = "EntityStats";
        _stats.Died += OnDied;
        AddChild(_stats);
    }

    public EntityStats GetEntityStats() => _stats;

    // --- chara initialization ---
    protected override void Initialize()
    {
        // animation
        _animationTree = GetNode<AnimationTree>("./AnimationTree");
        _animationStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
        _animationTree.Active = true;
        // combat boxes
        _attackHitbox = GetNode<Hitbox>("Hitbox");
        _hurtbox = GetNode<Hurtbox>("Hurtbox");
        _hurtbox.AddChild(GetNode<CollisionShape3D>("CollisionShape3D").Duplicate());
        _hurtbox.HitReceived += TakeDamage;
        // init stats
        InitializeStats();
    }

    // --- chara animation control ---
    public BearState GetAnimationState() => State;

    private void _AnimationUpdate()
    {
        if (State is BearState.Attacking or BearState.Hurting)
        {
            return;
        }

        State = Velocity.Length() > 0.05f ? BearState.Walking : BearState.Idle;
    }

    // --- chara physics ---
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _AnimationUpdate();
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
        _attackHitbox.Disable();
    }

    // --- chara hurt ---
    public override void TakeDamage(int amount)
    {
        if (State is BearState.Hurting or BearState.Dead) return;

        base.TakeDamage(amount); // This emits the Damaged signal
        _stats.TakeDamage(amount);

        if (State is BearState.Attacking) return;

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
