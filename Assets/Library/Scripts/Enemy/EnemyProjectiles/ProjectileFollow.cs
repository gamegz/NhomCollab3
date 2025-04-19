using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFollow : EnemyProjectile
{
    public Transform target;
    [SerializeField] protected float turnSpeed;
    [SerializeField] protected float steerCorectionSpeed = 0.01f;

    public override void Update()
    {
        base.Update();
        LookAtTarget(target, turnSpeed);
    }

    public override void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.forward * _bulletSpeed, ForceMode.Force);

        if (rb.velocity.magnitude > _bulletSpeed * 100)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, _bulletSpeed);
        }

        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        if (Mathf.Abs(localVelocity.x) > 0)
        {
            localVelocity.x = Mathf.Lerp(localVelocity.x, 0, steerCorectionSpeed);
            rb.velocity = transform.TransformDirection(localVelocity);
        }
    }

    public void LookAtTarget(Transform target, float speed)
    {
        Vector3 dirToTarget = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * speed);
    }

    public void SetUp(Vector3 shootDir, GameObject owner, Transform followTarget)
    {
        target = followTarget;
        ChangeShootDir(shootDir);
        _owner = owner;
    }

}
