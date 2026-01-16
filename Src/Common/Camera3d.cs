using Godot;
using System;

public partial class Camera3d : Camera3D
{
    private Vector3 _actualPosition;
    private ManualController _controller;
    private Vector3 _globalOffset = new(1.16f - 1.063f, 7.3f - 0.047f, 18.249f - 4.965f);
    private ShaderMaterial _material;

    public event Action<Vector2> OnSubPixelCorrection;


    public override void _Ready()
    {
        _controller = GetNode<ManualController>("../ManualController");
        Position = (_controller.Mob.Position + _globalOffset).Round();
        _actualPosition = (_controller.Mob.Position + _globalOffset).Round();
        _material = GetNode<SubViewportContainer>("../../../SubViewportContainer").Material as ShaderMaterial;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (IsInstanceValid(_controller.Mob))
        {
            _actualPosition = _actualPosition.Lerp(_controller.Mob.Position + _globalOffset, 10.0f * (float)delta);
            Vector3 localOffset3d = _actualPosition.Round() - _actualPosition;
            Vector2 localOffset2d = new Vector2(localOffset3d.X, localOffset3d.Z / 2) * 90.0f;
            _material.SetShaderParameter("CamOffset", localOffset2d);
            OnSubPixelCorrection?.Invoke(localOffset2d);
            Position = _actualPosition.Round();
        }
    }
}
