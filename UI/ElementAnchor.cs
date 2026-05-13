using Godot;

namespace ApexOverride.UI;

public partial class ElementAnchor : Control
{
    private Vector2 _currentSubPixelCorrection;
    [Export] public CameraPix GameCamera; // The Camera to unproject with
    [Export] public float PixelScale = 4.0f; // 1080 / 270

    [ExportGroup("Targets")] [Export] public Node3D TrackedNode; // The Bear/Mob

    [ExportGroup("Settings")] [Export] public Vector3 WorldOffset = new Vector3(0, 1.8f, 0); // Above head

    public void Bind(Node3D target, CameraPix camera)
    {
        TrackedNode = target;
        GameCamera = camera;
    }

    public override void _Ready()
    {
        if (GameCamera != null)
        {
            CameraPix.OnPixelCorrection += UpdateCorrection;
        }
    }

    public override void _ExitTree()
    {
        if (GameCamera != null)
        {
            CameraPix.OnPixelCorrection -= UpdateCorrection;
        }
    }

    private void UpdateCorrection(Vector2 offset)
    {
        _currentSubPixelCorrection = new Vector2(-offset.X, offset.Y);
    }

    public override void _Process(double delta)
    {
        // 1. Safety Checks
        if (!IsInstanceValid(TrackedNode) || !IsInstanceValid(GameCamera))
        {
            Visible = false;
            return;
        }

        Vector3 targetPos3D = TrackedNode.GlobalPosition + WorldOffset;

        // 2. Hide if behind camera
        if (GameCamera.IsPositionBehind(targetPos3D))
        {
            Visible = false;
            return;
        }

        Visible = true;

        // 3. Project to Low-Res Screen Space (e.g. 0 to 320)
        // Since CameraPix is "Physically Snapped", Unproject returns snapped coordinates.
        Vector2 lowResPos = GameCamera.UnprojectPosition(targetPos3D);

        // 4. Apply Sub-Pixel Smoothing
        // We add the error back to make the UI move smoothly between grid points
        // NOTE: Verify the sign (+/-) visually. Usually opposite to Camera move.
        Vector2 smoothLowRes = lowResPos - _currentSubPixelCorrection;

        // 5. Scale up to High-Res Screen Space
        GlobalPosition = smoothLowRes * PixelScale;
    }
}
