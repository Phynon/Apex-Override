using Godot;

public partial class ProjectileBase : Area3D
{
    public float Speed { get; set; } = 50.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GlobalTranslate(GlobalTransform.Basis * Vector3.Forward * Speed * (float)delta);
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print($"Hit body: {body.Name}");
        // Add logic here: deal damage, apply force, etc.
        // Example: If the body is an enemy, call a damage function
        // if (body is Enemy enemy) 
        // {
        // 		enemy.TakeDamage(10);
        // }
        QueueFree();
    }

    // This method is called when the projectile enters another Area3D
    private void OnAreaEntered(Area3D area)
    {
        GD.Print($"Hit area: {area.Name}");
        // Destroy the projectile
        QueueFree();
    }
}
