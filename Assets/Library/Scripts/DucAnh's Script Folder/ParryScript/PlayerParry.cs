using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : PlayerActionState
{
    #region Non-Serializable
    /* BOOLEAN VALUES */
    private bool canParry = true;
    private bool invulnerable = false;
    private bool standingStill = false;

    /* NUMERICAL VALUES */
    private float delayTimer;
    private float invulnerableTimer;
    private float standStillTimer;

    /* RFERENCES */
    private Rigidbody _rb;

    #endregion


    #region Serializable
    [Header("References")]
    [SerializeField] private Transform parryBoxRef;
    [SerializeField] private Vector3 parryBoxSize = new Vector3(1, 1, 1); // Default value
    [SerializeField] private LayerMask bulletLayerMask;


    [Header("Values")]
    [SerializeField] private float delayTimerOrg;
    [SerializeField] private float invulnerableTimerOrg;
    [SerializeField] private float standStillTimerOrg;

    #endregion


    private void Awake() // Setting up values
    {
        delayTimer = delayTimerOrg;
        invulnerableTimer = invulnerableTimerOrg;
        standStillTimer = standStillTimerOrg;
        _rb= GetComponent<Rigidbody>();
    }

    void Update()
    {
        #region Processor
        if (!canParry)
        {
            delayTimer -= Time.deltaTime;  

            if (delayTimer < 0)
            {
                canParry = true;
                delayTimer = delayTimerOrg;
            }
        }

        if (invulnerable)
        {
            invulnerableTimer -= Time.deltaTime;    

            if (invulnerableTimer <= 0)
            {
                invulnerable = false;
                invulnerableTimer = invulnerableTimerOrg;
            }
        }

        if (standingStill)
        {
            standStillTimer -= Time.deltaTime;   

            if (standStillTimer <= 0)
            {
                standingStill = false;
                standStillTimer = standStillTimerOrg;
            }
        }
        #endregion

        #region Player Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canParry)
            {
                canParry = false;   
                invulnerable = true;
                standingStill = true;

                StartCoroutine(Parrying());
                MoveToState(PlayerState.Parrying);

            }
        }
        #endregion

        switch (_state)
        {
            case PlayerState.Parrying:
                if (!standingStill) { MoveToState(PlayerState.Idle); }
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case PlayerState.Parrying:
                ParryingAction();
                break;
        }

    }

    protected override void ParryingAction()
    {
        base.ParryingAction();
        
        StandingStill();
    }


    IEnumerator Parrying()
    {
        while (invulnerable)
        {
            Collider[] colliders = Physics.OverlapBox(parryBoxRef.position, parryBoxSize / 2, transform.rotation, bulletLayerMask);

            if (colliders.Length > 0)
            {

                for (int i = 0; i < colliders.Length; i++)
                {
                    GameObject actualBullet = colliders[i].transform.parent.gameObject; // Why the fuck does it scan the children object instead of the parent...

                    EnemyProjectile bulletScript = actualBullet.GetComponent<EnemyProjectile>();

                    if (bulletScript != null)
                    {
                        bulletScript.ReflectBulletReverse();
                    }
                    else
                    {
                        Debug.Log("Something wrong with calling the script from bulletScript");
                    }

                }
            }
            Debug.Log("Length: " + colliders.Length);

            yield return null;

        }

    }


    private void StandingStill()
    {
        if (standingStill)
        {
            _rb.velocity = Vector3.zero;
        }
    }


    void OnDrawGizmos()
    {
        if (parryBoxRef != null)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(parryBoxRef.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, parryBoxSize);
        }
    }
}
