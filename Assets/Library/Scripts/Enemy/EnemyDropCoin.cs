using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropCoin : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinDropCount = 3;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Test
        {
            Die();
        }
    }

    private void Die()
    {
        DropCoins();
        Destroy(gameObject);
    }

    private void DropCoins()
    {
        for (int i = 0; i < coinDropCount; i++)
        {
            // Spawn each coin with a small offset for better visual effect
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }










    //// ONLY FOR OnDestroy()
    //// This prevents drops from spawning when the application is quitting or the scene is changing.
    // private bool isQuitting;
    //void OnApplicationQuit()
    //{
    //    isQuitting = true;
    //}
    //private void OnDestroy()
    //{
    //    if (!isQuitting)
    //    {
    //        Instantiate(coinPrefab, transform.position, Quaternion.identity);

    //    }
    //}
}
