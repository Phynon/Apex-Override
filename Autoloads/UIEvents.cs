using ApexOverride.Common;
using Godot;

namespace ApexOverride.Autoloads;

public partial class UIEvents : Node
{
    [Signal]
    public delegate void DamageNumberRequestedEventHandler(Vector3 pos, int amount);

    [Signal]
    public delegate void HealthBarRequestedEventHandler(MobBase mob, EntityStats stats);

    public static UIEvents Bus { get; private set; }

    public override void _Ready() => Bus = this;
}
