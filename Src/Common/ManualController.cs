using ApexOverride.Common;
using Godot;

public partial class ManualController : Node
{
    [Export] public MobBase Mob;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Mob?.Move(inputDir, delta);
        if (Input.IsActionJustPressed("ui_accept"))
        {
            Mob?.Attack();
        }
    }
}
