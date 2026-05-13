using ApexOverride.Autoloads;
using ApexOverride.Common;
using ApexOverride.Interfaces;
using Godot;

namespace ApexOverride.UI;

public partial class UIManager : CanvasLayer
{
    [Export] public CameraPix Camera { get; private set; }

    [ExportGroup("UI Element Scenes")]
    [Export]
    public PackedScene HealthBarScene { get; set; }
    // [Export] public PackedScene DamageNumberScene { get; set; }

    public override void _Ready()
    {
        UIEvents.Bus.HealthBarRequested += OnHealthBarRequested;
        // UIEvents.Bus.DamageNumberRequested += OnDamageNumberRequested;
        CatchUpExistingMobs();
    }

    public override void _ExitTree()
    {
        if (UIEvents.Bus != null)
        {
            UIEvents.Bus.HealthBarRequested -= OnHealthBarRequested;
            // UIEvents.Bus.DamageNumberRequested -= OnDamageNumberRequested;
        }
    }

    // ─── Event handlers (one line each, all pattern identical) ───

    private void OnHealthBarRequested(MobBase mob, EntityStats stats)
    {
        ElementAnchor anchor = SpawnAnchor(HealthBarScene, mob);
        // If the health bar widget needs extra setup, call it:
        if (anchor.GetChild(0) is HealthBar2D bar)
            bar.Setup(stats);
    }

    // private void OnDamageNumberRequested(Vector3 pos, int amount)
    // {
    //     ElementAnchor anchor = SpawnDetached(DamageNumberScene, pos);
    //     if (anchor.GetChild(0) is DamageNumber dmg)
    //         dmg.Show(amount); // auto-QueueFree after animation
    // }

    // ─── Spawning helpers ──────────────────────────────────────

    private ElementAnchor SpawnAnchor(PackedScene scene, Node3D target)
    {
        if (scene == null) return null;

        ElementAnchor anchor = new ElementAnchor();
        anchor.Bind(target, Camera);
        AddChild(anchor);

        Node widget = scene.Instantiate();
        anchor.AddChild(widget);

        return anchor;
    }

    private ElementAnchor SpawnDetached(PackedScene scene, Vector3 worldPos)
    {
        if (scene == null) return null;

        ElementAnchor anchor = new ElementAnchor();
        anchor.GameCamera = Camera;
        AddChild(anchor);
        anchor.GlobalPosition = Camera.UnprojectPosition(worldPos);

        Node widget = scene.Instantiate();
        anchor.AddChild(widget);

        return anchor;
    }

    private void CatchUpExistingMobs()
    {
        foreach (Node node in GetTree().GetNodesInGroup("Players"))
        {
            if (node is MobBase mob and IStatsBearer bearer)
            {
                OnHealthBarRequested(mob, bearer.GetEntityStats());
            }
        }
    }
}
