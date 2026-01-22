using Godot;

namespace ApexOverride.UI;

// TODO: decouple!

public partial class HealthBar2D : Control
{
    // IMPORTANT: Set this to match your SubViewportContainer "Stretch Shrink" or Scale.
    // If you render 320x180 on a 1280x720 screen, this is 4.0f.
    private const float PixelScale = 6.0f;
    private TextureProgressBar _bar;
    private Camera3d _camera; // Store the reference
    private Vector2 _currentDisplacement = Vector2.Zero;
    private EntityStats _stats;
    private Node3D _targetNode;

    public void Initialize(Node3D target, EntityStats stats, Camera3d camera)
    {
        _targetNode = target;
        _stats = stats;
        _camera = camera; // 1. Receive the camera from the Bear

        if (_bar == null) SetupProgressBar();

        _bar.MaxValue = _stats.MaxHealth;
        _bar.Value = _stats.CurrentHealth;
        _stats.HealthChanged += OnHealthChanged;
        _camera.OnSubPixelCorrection += _updateDisplacement;
        OnHealthChanged(_stats.CurrentHealth);
    }

    private void _updateDisplacement(Vector2 offset)
    {
        _currentDisplacement = offset;
    }

    public override void _Process(double delta)
    {
        if (_targetNode == null || _camera == null) return;

        // 2. Get position in the "Tiny" (SubViewport) world
        Vector2 lowResPos =
            _camera.UnprojectPosition(
                _targetNode.GlobalPosition + Vector3.Up * 1.8f + Vector3.Right * 3.0f / PixelScale);

        // Hide if behind camera
        if (_camera.IsPositionBehind(_targetNode.GlobalPosition))
        {
            Visible = false;
            return;
        }

        Visible = true;

        // 3. Scale UP to the "Big" (CanvasLayer) world
        // We Round() the lowResPos first to ensure it snaps to the game's pixel grid.
        var snappedX = Mathf.Round(lowResPos.X) * PixelScale;
        var snappedY = Mathf.Round(lowResPos.Y) * PixelScale;

        // Apply position centered on the pivot
        Position = new Vector2(snappedX, snappedY) - (_bar.PivotOffset * PixelScale) + _currentDisplacement;
    }

    // ... SetupProgressBar and OnHealthChanged remain the same ...
    private void SetupProgressBar()
    {
        _bar = new TextureProgressBar();
        var img = Image.CreateEmpty(1, 1, false, Image.Format.Rgba8);
        img.Fill(Colors.White);
        var tex = ImageTexture.CreateFromImage(img);

        _bar.NinePatchStretch = true;
        _bar.TextureUnder = tex;
        _bar.TextureProgress = tex;
        _bar.TintUnder = new Color(0.2f, 0.0f, 0.0f);
        _bar.TintProgress = Colors.Green;
        _bar.TextureFilter = TextureFilterEnum.Nearest;

        _bar.CustomMinimumSize = new Vector2(15, 2);
        _bar.PivotOffset = _bar.CustomMinimumSize / 2;
        _bar.Scale = new Vector2(PixelScale, PixelScale); // Keep this to make the bar chunky

        AddChild(_bar);
    }

    private void OnHealthChanged(int health)
    {
        if (_bar == null) return;
        _bar.Value = health;
        if (_bar.MaxValue > 0)
        {
            float pct = (float)health / (float)_bar.MaxValue;
            _bar.TintProgress = new Color(1.0f - pct, pct, 0);
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _camera.OnSubPixelCorrection -= _updateDisplacement;
    }
}
