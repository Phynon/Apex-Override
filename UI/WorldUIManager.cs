using ApexOverride.Autoloads;
using ApexOverride.Common;
using ApexOverride.Interfaces;
using Godot;

namespace ApexOverride.UI;

public partial class WorldUIManager : CanvasLayer
{
    [Export] public PackedScene HealthBarScene { get; set; }
    [Export] public Camera3d Camera { get; private set; }

    public override void _Ready()
    {
        // manually trigger health bar creation for existing mobs
        var existingMobs = GetTree().GetNodesInGroup("Players");
        foreach (Node node in existingMobs)
        {
            if (node is MobBase mob and IStatsBearer bearer)
            {
                CreateBar(mob, bearer.GetEntityStats());
            }
        }

        UIEvents.Bus.HealthBarRequested += CreateBar;
    }

    public override void _ExitTree()
    {
        // Clean up connection
        if (UIEvents.Bus != null)
        {
            UIEvents.Bus.HealthBarRequested -= CreateBar;
        }
    }

    private void CreateBar(MobBase mob, EntityStats stats)
    {
        if (HealthBarScene == null)
        {
            return;
        }

        HealthBar2D bar = HealthBarScene.Instantiate<HealthBar2D>();
        bar.Initialize(mob, stats, Camera);

        AddChild(bar);
        mob.TreeExiting += () => bar.QueueFree();
    }
}
