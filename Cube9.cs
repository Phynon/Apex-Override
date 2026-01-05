using ApexOverride.Interfaces;
using Godot;

public partial class Cube9 : MeshInstance3D, IDamageable
{
    public void TakeDamage(int amount)
    {
        GD.Print($"Cube9 took {amount} damage!");
        QueueFree();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
