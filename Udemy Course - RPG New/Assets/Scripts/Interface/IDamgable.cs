using UnityEngine;

public interface IDamgable
{

    public bool TakeDamage(float damage,float elementalDamage, Transform damageDealer);
}
