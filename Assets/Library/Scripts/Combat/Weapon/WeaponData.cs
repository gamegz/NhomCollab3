using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Scriptable Object", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int baseDamage;
    public int critDamageMultiplier;
    public int critChance;
    public int attackSpeed;
    public int attackRecoverSpeed;
    public int attackStaggerForce;

}
