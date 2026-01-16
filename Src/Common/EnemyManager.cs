using Godot;
using Godot.Collections;

// Namespace for your Bear/MobBase

public partial class EnemyManager : Node3D
{
    private Camera3D _camera;

    private Timer _spawnTimer;
    [Export] public int MaxRetries = 10; // How many times to try finding a hidden spot
    [Export] public float SpawnInterval = 20.0f;
    [Export] public PackedScene EnemyScene { get; set; }

    // Drag your Area3D nodes here in the Inspector
    [Export] public Array<Area3D> SpawnZones { get; set; }

    public override void _Ready()
    {
        _spawnTimer = new Timer();
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.Timeout += AttemptSpawn;
        AddChild(_spawnTimer);
        _spawnTimer.Start();
    }

    private void AttemptSpawn()
    {
        if (EnemyScene == null || SpawnZones.Count == 0) return;

        // Lazy load the camera (it might not be ready at _Ready)
        if (_camera == null) _camera = GetViewport().GetCamera3D();
        if (_camera == null) return;

        // Try to find a valid point
        for (int i = 0; i < MaxRetries; i++)
        {
            // 1. Pick Random Zone
            Area3D zone = SpawnZones[GD.RandRange(0, SpawnZones.Count - 1)];

            // 2. Pick Random Point in Zone
            Vector3 candidatePos = GetRandomPointInArea(zone);

            // 3. Check Visibility
            if (!IsPositionVisible(candidatePos))
            {
                SpawnEnemy(candidatePos);
                return; // Success! Stop trying.
            }
        }

        // If we reach here, we failed 10 times. 
        // Optional: Force spawn anyway, or just wait for next tick.
    }

    private void SpawnEnemy(Vector3 position)
    {
        Node3D enemy = EnemyScene.Instantiate<Node3D>();
        AddChild(enemy);
        enemy.GlobalPosition = position;

        // Optional: Random rotation
        enemy.RotateY(GD.Randf() * Mathf.Tau);
    }

    // --- HELPER: Random Math ---
    private Vector3 GetRandomPointInArea(Area3D area)
    {
        // We assume the shape is a BoxShape3D for simplicity
        var collisionShape = area.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
        if (collisionShape?.Shape is BoxShape3D box)
        {
            Vector3 extents = box.Size / 2.0f;
            float randX = (float)GD.RandRange(-extents.X, extents.X);
            float randZ = (float)GD.RandRange(-extents.Z, extents.Z);

            // Convert local box point to global world point
            return area.ToGlobal(new Vector3(randX, 0, randZ));
        }

        // Fallback: Just return the center
        return area.GlobalPosition;
    }

    // --- HELPER: Visibility Check ---
    private bool IsPositionVisible(Vector3 globalPos)
    {
        // 1. Check if behind the camera (Simple Dot Product)
        if (_camera.IsPositionBehind(globalPos))
            return false; // Definitely not visible

        // 2. Check if inside the screen rectangle
        // Unproject converts 3D World Pos -> 2D Screen Pos
        Vector2 screenPos = _camera.UnprojectPosition(globalPos);
        Rect2 viewportRect = GetViewport().GetVisibleRect();

        // Add a small margin so they don't pop in right at the edge
        // Expanding the rect means "Visible" includes a buffer zone around the screen
        Rect2 paddedRect = viewportRect.Grow(10);

        return paddedRect.HasPoint(screenPos);
    }
}
