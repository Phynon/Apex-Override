using ApexOverride.Common;
using Godot;

namespace ApexOverride.Autoloads;

public partial class CombatEvents : Node
{
    [Signal]
    public delegate void FactionAggroTriggeredEventHandler(int factionId, Vector3 origin);

    [Signal]
    public delegate void MobDiedEventHandler(MobBase mob);

    public static CombatEvents Bus { get; private set; }

    public override void _Ready() => Bus = this;
}
