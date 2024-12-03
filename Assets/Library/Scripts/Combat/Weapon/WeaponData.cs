using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Object", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    // New Variable Structure (PLEASE FOLLOW):
    // [Tooltip("What it Does")]
    // public T name = <Default Value Here>;

    public string weaponName;
    public int critDamageMultiplier;
    public int critChance;
    public float attackSpeed;
    public float chargeAttackSpeed;
    public float holdThreshold;
    public int baseWeaponDamage;
    public int chargeWeaponDamage;
    public float parryDuration;
    public float parrySpeed;

    [Tooltip("Force applied to IDamabage when damaged below threshhold")]
    public float knockbackForce = 5;
}
