using Godot;

namespace ApexOverride.Common;

public partial class Hurtbox : Area3D
{
    [Signal]
    public delegate void HitReceivedEventHandler(int damage);

    public override void _Ready()
    {
        CollisionLayer = 4; // Exists to be hit
        CollisionMask = 0;
    }

    public void ReceiveHit(int damage)
    {
        EmitSignal(SignalName.HitReceived, damage);
    }
}
