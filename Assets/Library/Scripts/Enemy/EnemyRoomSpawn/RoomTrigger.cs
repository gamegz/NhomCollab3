using Core.Events;
using Enemy;
using Enemy.EnemyManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private GameObject RoomEntranceDoor;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boxCollider.enabled = false;
            RoomEntranceDoor.SetActive(true);
            if(enemySpawner == null) { return; }
            enemySpawner.SetActive(true);
        }
    }
}
