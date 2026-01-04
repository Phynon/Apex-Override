using Godot;

public partial class Camera3d : Camera3D
{
    // Called when the node enters the scene tree for the first time.
    private ManualController _controller;

    public override void _Ready()
    {
        _controller = GetNode<ManualController>("../ManualController");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Position = _controller.Mob.Position + new Vector3(1.16f - 1.063f, 7.3f - 0.047f, 18.249f - 4.965f);
    }
}
