using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonHandProjectile : ProjectileFollow
{
    [SerializeField] private float followDelayTime = 2;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void Update()
    {
        UpdateLifeTime();
        followDelayTime -= Time.deltaTime;
        if (followDelayTime > 0) { return; }
        LookAtTarget(player, turnSpeed);
                      
    }

    public override void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.forward * _bulletSpeed, ForceMode.Force);

        if (rb.velocity.magnitude > _bulletSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, _bulletSpeed);
        }

        if (followDelayTime > 0) { return; }
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        if (Mathf.Abs(localVelocity.x) > 0)
        {
            localVelocity.x = Mathf.Lerp(localVelocity.x, 0, steerCorectionSpeed);
            rb.velocity = transform.TransformDirection(localVelocity);
        }
    }

    public override void OnTriggerEnter(Collider other)
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
    }
}
