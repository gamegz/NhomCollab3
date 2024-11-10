using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    #region Non-Serializable
    [Header("Values")]
    private bool canParry = true;
    private float delayTimer;
    #endregion


    #region Serializable
    [Header("References")]
    [SerializeField] private Transform parryBoxRef;
    [SerializeField] private Vector3 parryBoxSize = new Vector3(1, 1, 1); // Default value
    [SerializeField] private LayerMask bulletLayerMask;


    [Header("Values")]
    [SerializeField] private float delayTimerOrg;
    #endregion


    private void Start()
    {
        delayTimer = delayTimerOrg;
    }

    void Update()
    {
        #region DELAY TIMER
        if (!canParry)
        {
            delayTimer -= Time.deltaTime;

            if (delayTimer < 0)
            {
                canParry = true;
                delayTimer = delayTimerOrg;
            }
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canParry)
            {
                Parrying();
                canParry = false;   
            }
        }
    }


    void Parrying()
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

                Debug.Log("Layers: " + colliders[i].gameObject.layer.ToString());
            }
        }
        else { return; }

        Debug.Log("Length of array: " + colliders.Length);

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
