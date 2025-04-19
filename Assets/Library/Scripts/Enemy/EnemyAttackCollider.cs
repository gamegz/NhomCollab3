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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_playerTagName))
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(_damage);
        }
    }

}
