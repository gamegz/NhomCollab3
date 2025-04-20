using Core.Events;
using Enemy;
using Enemy.EnemyManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private GameObject Door1;
    [SerializeField] private GameObject Door2;
    [SerializeField] private GameObject Door3;
    [SerializeField] private GameObject Door4;
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
            if (Door1 != null)
            {
                Door1.SetActive(true);
            }

            if (Door2 != null)
            {
                Door2.SetActive(true);
            }

            if (Door3 != null)
            {
                Door3.SetActive(true);
            }

            if (Door4 != null)
            {
                Door4.SetActive(true);
            }
            if(enemySpawner == null) { return; }
            enemySpawner.SetActive(true);
        }
    }
}
