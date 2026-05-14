using Godot;

namespace ApexOverride.Common;

public partial class Hitbox : Area3D
{
    [Signal]
    public delegate void HitLandedEventHandler(Hurtbox target);

    private int _currentActiveDamage;

    private CollisionShape3D _shape;

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = 4; // Scans Hurtboxes

        _shape = GetNode<CollisionShape3D>("CollisionShape3D");
        AreaEntered += OnAreaEntered;
    }

    public void Enable(int damage)
    {
        _currentActiveDamage = damage;
        _shape.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
    }

    public void Disable()
    {
        _shape.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
        _currentActiveDamage = 0;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (area is Hurtbox hurtbox)
        {
            if (hurtbox.Owner == Owner)
            {
                return;
            }

            hurtbox.ReceiveHit(_currentActiveDamage);
        }
    }
}
