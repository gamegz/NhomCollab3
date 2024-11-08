using Core.Events;
using Enemy.EnemyManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    EnemyManager enemyManager;

    void Start()
    {

    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //enemyManager.OnSpawnRequest()
        }
    }
}
