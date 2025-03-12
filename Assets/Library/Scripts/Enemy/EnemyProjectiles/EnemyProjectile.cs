using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Handle colllision logic and bouncing to shooter
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;

    protected GameObject _owner;
    protected Vector3 _shootDir = Vector3.forward;
    protected bool _deflected = false;

    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _reflectedDamage;
    [SerializeField] protected float _lifeTime;

    public ProjectileDeflectMethod projectileDeflectMethod;

    public virtual void Update()
    {              
        UpdateLifeTime();
    }

    public virtual void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.forward * _bulletSpeed, ForceMode.Impulse);

        if (rb.velocity.magnitude > _bulletSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, _bulletSpeed);
        }
    }

    protected void UpdateLifeTime()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReflectBulletReverse()
    {
        if(_deflected) { return; }
        switch (projectileDeflectMethod)    
        {
            case ProjectileDeflectMethod.NoDeflect:
                break;
            case ProjectileDeflectMethod.OppositeDirection:
                ChangeShootDir(-_shootDir); _deflected = true;
                break;
            case ProjectileDeflectMethod.BackToOwner:
                ReflectBulletToOwner();_deflected = true;
                break;
        }
        
    }

    private void ReflectBulletToOwner()
    {
        if(_owner == null) { return; }
        ChangeShootDir(_owner.transform.position - transform.position);
        _deflected = true;
    }

    //Call when spawn
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


    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !_deflected)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(_damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && _deflected)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(_reflectedDamage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum ProjectileDeflectMethod
    {
        NoDeflect,
        OppositeDirection,
        BackToOwner
    }
}
