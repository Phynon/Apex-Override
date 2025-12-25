using Godot;

public partial class MobBase : Node
{
    public int Health = 100;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void _TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            QueueFree();
        }
    }
}
