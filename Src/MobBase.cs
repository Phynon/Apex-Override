using Godot;
using System;

public partial class MobBase : Node
{
	public int health = 100;
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
		health -= amount;
		if (health <= 0)
		{
			QueueFree();
		}
	}
}
