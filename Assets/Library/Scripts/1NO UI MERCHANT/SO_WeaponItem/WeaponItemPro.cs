using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Weapon Item Pro", menuName = "ScriptableObjects/Item/Weapon Item Pro")]
public class WeaponItemPro : ScriptableObject
{
    public GameObject weaponPrefab;
    public GameObject weaponModel;
    public string weaponName;
    public int weaponBioCurrencyCost;
    public int weaponCreditCurrencyCost;
}
