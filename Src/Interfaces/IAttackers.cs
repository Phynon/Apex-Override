namespace ApexOverride.Interfaces;

public interface IAttacker
{
    void Attack();
    void OnAttackHit();
}

public interface IMeleeAttacker : IAttacker
{
}
