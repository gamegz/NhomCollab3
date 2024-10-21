using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponBase
{

    public override void OnInnitNormalAttack()
    {
        Debug.Log("attack");
        Debug.Log(_weaponData.baseDamage);
    }

    public override void OnStopInnitNormalAttack()
    {
        Debug.Log("stop normal attack");
    }

    public override void OnInnitSecondaryAttack()
    {
        
    }
}
