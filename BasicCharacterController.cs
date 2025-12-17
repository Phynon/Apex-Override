using Godot;
using System;

public partial class BasicCharacterController : CharacterBody3D
{
	public const float Speed = 1.0f;
	public const float JumpVelocity = 4.5f;
	public enum PlayerState
	{
		Idle,
		Walking,
		Jumping
	}
	PlayerState playerState;
	
	public AnimationTree _AnimationStateMachine;

	public override void _Ready()
	{
		_AnimationStateMachine = GetNode<AnimationTree>("./AnimationTree");
		_AnimationStateMachine.Active = true;
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
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			double _targetAngle = Mathf.Atan2(inputDir.X, inputDir.Y);
			velocity.X = inputDir.X * Speed;
			velocity.Z = inputDir.Y * 2 * Speed;
			Rotation = new Vector3(Rotation.X, (float)Mathf.LerpAngle(Rotation.Y, _targetAngle + 0.50f * Mathf.Pi, 20.0f * delta), Rotation.Z);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, 2 * Speed);
		}
		Velocity = velocity;
		_AnimationUpdate();
		GD.Print(playerState);
		MoveAndSlide();
	}
	
	public void _AnimationUpdate()
	{
		if (Velocity != Vector3.Zero)
		{
			playerState = PlayerState.Walking;
		}
		else
		{
			playerState = PlayerState.Idle;
		}
	}
	
	public PlayerState GetAnimationState() => playerState;
}
