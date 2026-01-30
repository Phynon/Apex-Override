using Godot;
using System;

public partial class CameraPix : Camera3D
{
    private const float ViewportScale = 4.0f;
    private const float ViewportHeight = 270.0f;
    private ManualController _controller;
    private Vector3 _debugCursor;
    private Vector3 _globalOffset = new(0.0f, 16.0f, 30.0f);
    private ShaderMaterial _material;
    private Transform3D _snapSpace;
    private Vector3 _targetPosition;

    [Export] public bool DebugFreeCam = false;
    [Export] public float DebugSpeed = 5.0f;

    public static event Action<Vector2> OnPixelCorrection;

    public override void _Ready()
    {
        _debugCursor = Position;
        _controller = GetNode<ManualController>("../../World/ManualController");
        _snapSpace = GlobalTransform;
        if (IsInstanceValid(_controller?.Mob))
        {
            Vector3 mobPosition = _controller.Mob.Position + _globalOffset;
            _targetPosition = mobPosition;
            SnapCamera(_targetPosition);
        }
    }

    private void SnapCamera(Vector3 targetPosition)
    {
        Position = targetPosition;
        float upp = Size / ViewportHeight;
        Vector3 idealPosition = Position * _snapSpace;
        Vector3 snappedPosition = idealPosition.Snapped(Vector3.One * upp);
        Vector3 offset3d = snappedPosition - idealPosition;
        Vector2 offset = new Vector2(offset3d.X, offset3d.Y);
        HOffset = offset.X;
        VOffset = offset.Y;
        OnPixelCorrection?.Invoke(offset / upp);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (DebugFreeCam)
        {
            // Simple WASD check (works even if Input Map isn't set up)
            Vector3 inputDir = Vector3.Zero;
            if (Input.IsKeyPressed(Key.W)) inputDir.Z -= 1;
            if (Input.IsKeyPressed(Key.S)) inputDir.Z += 1;
            if (Input.IsKeyPressed(Key.A)) inputDir.X -= 1;
            if (Input.IsKeyPressed(Key.D)) inputDir.X += 1;

            if (inputDir.Length() > 0)
            {
                _debugCursor.X += inputDir.X * DebugSpeed * (float)delta;
                _debugCursor.Z += inputDir.Z * 2 * DebugSpeed * (float)delta;
            }

            _targetPosition = _targetPosition.Lerp(_debugCursor, 3.0f * (float)delta);
            SnapCamera(_debugCursor);
        }
        else if (IsInstanceValid(_controller.Mob))
        {
            // TODO: still minor jitters on health bar to fix
            Vector3 mobPosition = _controller.Mob.Position + _globalOffset;
            _targetPosition = _targetPosition.Lerp(mobPosition, 3.0f * (float)delta);
            SnapCamera(_targetPosition);
        }
    }
}
