using Godot;

public enum PlayerState
{
    Idle,
    Walking,
    Jumping
}

public partial class BasicCharacterController : CharacterBody3D
{
    private AnimationTree _animationStateMachine;
    public float Speed { get; set; } = 1.0f;
    // public const float JumpVelocity = 4.5f;
    public PlayerState PlayerState { get; set; }

    public override void _Ready()
    {
        _animationStateMachine = GetNode<AnimationTree>("./Bear/AnimationTree");
        _animationStateMachine.Active = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        // if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        // {
        // 		velocity.Y = JumpVelocity;
        // }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            double targetAngle = Mathf.Atan2(inputDir.X, inputDir.Y);
            velocity.X = inputDir.X * Speed;
            velocity.Z = inputDir.Y * 2 * Speed;
            Rotation = new Vector3(Rotation.X,
                (float)Mathf.LerpAngle(Rotation.Y, targetAngle + 0.50f * Mathf.Pi, 20.0f * delta), Rotation.Z);
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, 2 * Speed);
        }

        Velocity = velocity;
        _AnimationUpdate();
        MoveAndSlide();
    }

    public void _AnimationUpdate()
    {
        PlayerState = Velocity == Vector3.Zero ? PlayerState.Idle : PlayerState.Walking;
    }

    public PlayerState GetAnimationState() => PlayerState;
}
