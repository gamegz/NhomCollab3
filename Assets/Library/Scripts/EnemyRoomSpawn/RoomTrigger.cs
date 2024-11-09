using Core.Events;
using Enemy;
using Enemy.EnemyManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject enemySpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemySpawner.SetActive(true);
        }
    }
}
