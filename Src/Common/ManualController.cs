using ApexOverride.Common;
using ApexOverride.Interfaces;
using Godot;

public partial class ManualController : Node
{
    [Export] public MobBase Mob;

    public override void _Ready()
    {
        Node ai = Mob.GetNode("AIController");
        if (ai != null)
        {
            // "Disabled" stops _Process and _PhysicsProcess on that node
            ai.ProcessMode = ProcessModeEnum.Disabled;
            GD.Print("ManualController: AI Brain detected and disabled.");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(Mob))
        {
            return;
        }

        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Mob?.Move(inputDir, delta);
        if (Input.IsActionJustPressed("ui_accept"))
        {
            (Mob as IMeleeAttacker)?.Attack();
        }
    }
}
