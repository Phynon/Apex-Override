using Godot;

namespace ApexOverride.UI;

public partial class Screen : Sprite2D
{
    private ShaderMaterial _material;

    public override void _Ready()
    {
        _material = Material as ShaderMaterial;
        CameraPix.OnPixelCorrection += ApplySmoothing;
    }

    public override void _ExitTree()
    {
        CameraPix.OnPixelCorrection -= ApplySmoothing;
    }

    // This function runs automatically whenever the Camera fires the event
    private void ApplySmoothing(Vector2 offset)
    {
        _material?.SetShaderParameter("CamOffset", new Vector2(-offset.X, offset.Y));
    }
}
