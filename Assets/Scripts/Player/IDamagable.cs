using UnityEngine;

public interface IDamagable
{
    public delegate void DamageEventDelegate(IDamagable damageTaker, float damage, object cause, GameObject instigator);
    public event DamageEventDelegate OnDamaged;

    public void ApplyDamage(float damage, object cause, GameObject instigator);
}
