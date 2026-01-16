using ApexOverride.Common;
using Godot;

public partial class AIController : Node
{
    private float _aggroRange = 20.0f;

    private float _attackTimer = 0.0f;

    private float _stoppingDistance = 3.0f;
    [Export] public MobBase Pawn { get; set; }
    [Export] public Node3D Target { get; set; }

    public override void _Ready()
    {
        Pawn ??= GetParentOrNull<MobBase>();

        if (Target == Pawn)
        {
            Target = GetTree().GetFirstNodeInGroup("Players") as Node3D;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(Pawn) || !IsInstanceValid(Target))
        {
            return;
        }

        Vector3 toTarget = Target.GlobalPosition - Pawn.GlobalPosition;
        var distance = toTarget.Length();

        if (distance > _aggroRange)
        {
            // Idle behavior
            // TODO: change to wandering
            Pawn.Move(Vector2.Zero, delta);
            return;
        }

        if (_attackTimer > 0)
        {
            _attackTimer -= (float)delta;
        }

        if (distance > _stoppingDistance)
        {
            // Chase behavior
            // We map 3D direction to 2D input (X, Z) -> (X, Y)
            Vector2 inputDir = new Vector2(toTarget.X, toTarget.Z).Normalized();
            Pawn.Move(inputDir, delta);
        }
        else
        {
            // Attack behavior
            Pawn.Move(Vector2.Zero, delta); // Stop moving
            if (_attackTimer <= 0)
            {
                Pawn.Attack();
                _attackTimer = 2.0f;
            }
        }
    }
}
