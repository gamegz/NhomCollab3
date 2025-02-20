using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileCollider : MonoBehaviour
{
    private EnemyProjectile enemyProjectileScript;
    private bool deflected = false;

    private void Start()
    {
        enemyProjectileScript = gameObject.transform.parent.GetComponent<EnemyProjectile>();    
        if (enemyProjectileScript == null)
        {
            Debug.Log("Where is the enemy projectile script???");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !deflected)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(enemyProjectileScript.GetBulletDamage()); // Dealing 1 DMG to player (player has 5 hearts [could be expanded, I dont know])
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && deflected)
        {
            Debug.Log("HUH"); 
            other.gameObject.GetComponent<IDamageable>().TakeDamage(enemyProjectileScript.GetBulletDamage() * 10); // Dealing 10 DMG to enemies
            Destroy(gameObject);
        }
    }
}
