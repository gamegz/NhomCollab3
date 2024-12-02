using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : WeaponBase
{
    private bool _isChargeAttack = false;
    public override void OnInnitNormalAttack()
    {
        Debug.Log("PipeAttack");
        StartCoroutine(WaitToTurnOffBoxCollider());
    }

    public override void OnStopInnitNormalAttack()
    {

    }

    public override void OnInnitSecondaryAttack()
    {
        Debug.Log("PipeChargeAttack");
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
