using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : WeaponBase
{
    public override void OnInnitNormalAttack()
    {
        StartCoroutine(WaitToTurnOffBoxCollider());
    }

    public override void OnStopInnitNormalAttack()
    {
        
    }

    public override void OnInnitSecondaryAttack()
    {
        Debug.Log("chargeAttack");
        StartCoroutine(WaitToTurnOffBoxCollider());
    }

    IEnumerator WaitToTurnOffBoxCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.1f);
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
