using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : WeaponBase
{
    private bool _isChargeAttack = false;
    public override void OnInnitNormalAttack()
    {
        _isChargeAttack = false;
        StartCoroutine(WaitToTurnOffBoxCollider());
    }

    public override void OnStopInnitNormalAttack()
    {
        
    }

    public override void OnInnitSecondaryAttack()
    {
        Debug.Log("chargeAttack");
        _isChargeAttack = true;
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
            int damageDeal = _isChargeAttack ? _weaponData.chargeAttackDamage : _weaponData.baseDamage;
            damageable.TakeDamage(damageDeal);
        }
    }
}
