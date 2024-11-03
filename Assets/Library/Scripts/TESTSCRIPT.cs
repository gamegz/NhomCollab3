using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TESTSCRIPT : MonoBehaviour
{
    //This script is use to test small behavior to see if it work as intented
    //Note: I hate this
    //Second Note: I double hate this

    public GameObject target;

    void Start()
    {
        
    }

    //void Update()
    //{
    //    NavMeshPath navpath = new NavMeshPath();
    //    NavMesh.CalculatePath(transform.position, target.transform.position, -1, navpath);
    //    bool pathReachable = (navpath.status == NavMeshPathStatus.PathPartial || navpath.status == NavMeshPathStatus.PathInvalid) ? false : true;
    //    NavMeshHit hit;
    //    Debug.Log(enemyNavAgent.CalculatePath(dashPoint, enemyNavAgent.path));
    //}


}
