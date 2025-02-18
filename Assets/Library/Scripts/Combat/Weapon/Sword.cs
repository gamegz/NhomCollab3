using Enemy;
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
        Debug.Log("hit");
        GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.3f);
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemy)
        {
            enemy.DamagedByWeapon(_weaponData);
        }

        IDamageable damagable = other.GetComponent<IDamageable>();

        if (damagable != null && !other.CompareTag("Player"))
        {
            damagable.TakeDamage(_weaponData.baseWeaponDamage * PlayerDatas.Instance.GetStats.DamageModifier);
        }
    }
}
