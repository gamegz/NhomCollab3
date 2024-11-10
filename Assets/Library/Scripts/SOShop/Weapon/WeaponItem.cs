using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Weapon Item", menuName = "ScriptableObjects/Item/Weapon Item")]
public class WeaponItem : ScriptableObject 
{
    public GameObject weaponPrefab;
    public GameObject weapon3DModel;
    public string weaponName;
    public int weaponPrice;
    public Sprite weaponSprite;
}
