using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    void TakeDamage(int damageAmount);
    void Staggered(int timeAmount, float knockbackStrength, Vector3 weaponPos);
}
