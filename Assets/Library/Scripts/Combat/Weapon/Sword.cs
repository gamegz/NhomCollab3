using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponBase
{
    public override void OnInnitNormalAttack()
    {
        Debug.Log("attack");
        StartCoroutine(WaitToTurnOffBoxCollider());
    }

    public override void OnStopInnitNormalAttack()
    {
        //Debug.Log("stop normal attack");
    }

    public override void OnInnitSecondaryAttack()
    {
        Debug.Log("charge attack");
        StartCoroutine(WaitToTurnOffBoxCollider());
    }

    IEnumerator WaitToTurnOffBoxCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
       IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            
        }
    }
}
