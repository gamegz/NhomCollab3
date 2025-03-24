using Enemy;
using System.Collections;
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

        #region On Damage Enemy
        // Temp solution - Please update this in feature code by creating new Interfaces
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemy)
        {
            enemy.DamagedByWeapon(_weaponData);
        }
        #endregion

        IDamageable damagable = other.GetComponent<IDamageable>();

        if (damagable != null && !other.CompareTag("Player"))
        {
            damagable.TakeDamage(_weaponData.baseWeaponDamage);
        }
    }
}
