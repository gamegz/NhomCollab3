using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Scriptable Object", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int critDamageMultiplier;
    public int critChance;
    public float attackSpeed;
    public float chargeAttackSpeed;
    public int attackStaggerForce;
    public float holdThreshold;

}
