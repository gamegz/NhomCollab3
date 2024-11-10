using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handle colllision logic and bouncing to shooter
public class EnemyProjectile : MonoBehaviour
{    
    private GameObject _owner;
    private Vector3 _shootDir = Vector3.forward;

    [SerializeField] private float _bulletSpeed;
    [SerializeField] private int _damage;
    [SerializeField] private float _lifeTime;

    private void Update()
    {
        transform.position += _shootDir * _bulletSpeed * Time.deltaTime;

        _lifeTime -= Time.deltaTime;
        if(_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReflectBulletReverse()
    {
        ChangeShootDir(-_shootDir);
    }

    public void ReflectBulletToOwner() // Might not use this yet, will discuss with the team.
    {
        ChangeShootDir(_owner.transform.position - transform.position);
    }

    public void SetUp(Vector3 shootDir, GameObject owner)
    {
        ChangeShootDir(shootDir);
        _owner = owner;
    }

    private void ChangeShootDir(Vector3 shootDir)
    {
        shootDir = shootDir.normalized;
        Vector3 caculatedShootDir = new Vector3(shootDir.x, 0, shootDir.z);
        _shootDir = caculatedShootDir;
        transform.rotation = Quaternion.LookRotation(shootDir);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(_damage);
            Destroy(gameObject);
        }

    }



}
