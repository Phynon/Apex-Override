using ApexOverride.Interfaces;
using Godot;

namespace ApexOverride.Utils;

public abstract partial class MobBase : CharacterBody3D, IDamageable
{
    protected abstract float Speed { get; }
    protected abstract float RotateSpeed { get; }

    public void TakeDamage(int amount)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }


    public virtual void Move(Vector2 targetDirection, double delta)
    {
        Vector3 direction = (Transform.Basis * new Vector3(targetDirection.X, 0, targetDirection.Y)).Normalized();
        Vector3 velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        if (direction != Vector3.Zero)
        {
            // Standard movement math
            double targetAngle = Mathf.Atan2(targetDirection.X, targetDirection.Y);
            velocity.X = targetDirection.X * Speed;
            velocity.Z = targetDirection.Y * 2 * Speed;

            Rotation = new Vector3(Rotation.X,
                (float)Mathf.LerpAngle(Rotation.Y, targetAngle + 0.50f * Mathf.Pi, RotateSpeed * delta),
                Rotation.Z);
        }
        else
        {
            // Standard friction
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, 2 * Speed);
        }

        Velocity = velocity;
    }

    public abstract void Attack();
}
