using Godot;

public partial class EntityStats : Node
{
    [Signal]
    public delegate void DiedEventHandler();

    // --- SIGNALS (The wiring) ---
    // These allow the UI and the Wolf to react without being tightly coupled.
    [Signal]
    public delegate void HealthChangedEventHandler(int newHealth);


    [Export] public int MaxHealth = 100;
    [Export] public int CurrentHealth { get; private set; }

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    // The Wolf calls this method. The math happens here.
    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth -= amount;
        EmitSignal(SignalName.HealthChanged, CurrentHealth);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            EmitSignal(SignalName.Died);
        }
    }
}
