using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class PlayerParry : PlayerActionState
{
    #region Non-Serializable
    [Header("Values")]
    /* BOOLEAN VALUES */
    private bool canParry = true;
    private bool invulnerable = false;
    private bool standingStill = false;

    /* NUMERICAL VALUES */
    private float delayTimer;
    private float invulnerableTimer;
    private float standStillTimer;
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
    }

    void Update()
    {
        #region PARRY PROCESS
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canParry)
            {
                #region Setting Up Booleans
                canParry = false;   
                invulnerable = true;
                standingStill = true;
                #endregion

                #region Coroutines
                StartCoroutine(Parrying());

                #endregion
            }
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

        StartCoroutine(StandingStill());
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


    IEnumerator StandingStill()
    {
        while (standingStill)
        {
            PlayerMovement movementScript = this.gameObject.GetComponent<PlayerMovement>(); 

            if (movementScript != null)
            {
                //
            }
            else
            {
                Debug.Log("Couldn't find the player's Rigidbody component");
            }

            yield return null;
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
