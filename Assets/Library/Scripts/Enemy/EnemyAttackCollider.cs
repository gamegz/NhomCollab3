using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    public int _damage;
    [SerializeField] private string _playerTagName;

    private void Start()
    {
        if(_damage == 0)        
            Debug.LogWarning("Enemy collider damage unassign");
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(_playerTagName))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(_damage);
        }
    }

}
