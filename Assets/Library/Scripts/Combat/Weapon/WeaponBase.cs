using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public enum WeaponType
    {
        Light,
        Medium,
        Heavy
    }

    [Header("WEAPON CONFIG")] 
    public GameObject weaponModel;
    [SerializeField] protected LayerMask _layerData;
    public WeaponData _weaponData; //Make an SO with all it's atributes in it (See NOTION)
    /// <summary>
    /// Add anything you want the weapon to do before innitialization here
    /// Like model, effects, Dotween and stuff like that
    /// </summary>


    //Implement in Weapon Base child
    public abstract void OnInnitNormalAttack(); //Increase timer in here
    public abstract void OnStopInnitNormalAttack(); //Stop timer
    public abstract void OnInnitSecondaryAttack(); //
}
    
