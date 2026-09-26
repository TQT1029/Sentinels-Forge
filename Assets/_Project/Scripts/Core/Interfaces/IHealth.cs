using System;

// Context cục bộ cho damage
public struct DamageInfo
{
    public float damage;
    public bool isCritical;
}
public interface IHealth
{
    public void TakeDamage(DamageInfo info);
    public void Heal(float amount);
}
