using Godot;

namespace ApexOverride.UI;

public partial class HealthBar2D : Control
{
    private TextureProgressBar _bar;
    private EntityStats _stats;

    // Remove Initialize(). Use a Setup method that only takes what it needs.
    public void Setup(EntityStats stats)
    {
        _stats = stats;

        if (_bar == null) SetupProgressBar();

        _bar.MaxValue = _stats.MaxHealth;
        _bar.Value = _stats.CurrentHealth;

        // Unsubscribe first to avoid duplicates if called multiple times
        _stats.HealthChanged -= OnHealthChanged;
        _stats.HealthChanged += OnHealthChanged;

        OnHealthChanged(_stats.CurrentHealth);
    }

    private void SetupProgressBar()
    {
        // ... (Keep your exact existing SetupProgressBar code here) ...
        // Ensure you remove the "PixelScale" logic from here if you want the 
        // Anchor to handle scaling, OR keep it here if the bar internals need it.
        // For now, keeping your existing code is fine.
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

        _bar.CustomMinimumSize = new Vector2(25, 4);
        _bar.PivotOffset = _bar.CustomMinimumSize / 2;
        _bar.Scale = new Vector2(4, 4); // Keep this to make the bar chunky

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
}
