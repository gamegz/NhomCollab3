using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponBase
{

    public override void OnInnitNormalAttack()
    {
        //Debug.Log("attack");
        //Debug.Log(_weaponData.baseDamage);
        this.GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(WaitToTurnOffBoxCollider());
    }

    public override void OnStopInnitNormalAttack()
    {
        //Debug.Log("stop normal attack");
    }

    public override void OnInnitSecondaryAttack()
    {
        
    }

    IEnumerator WaitToTurnOffBoxCollider()
    {
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
