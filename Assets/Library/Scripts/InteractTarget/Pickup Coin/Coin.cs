using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private int bioGranted;
    private void Start()
    {
        CoinManager.RegisterCoin(this); // Automatically adds to the coins list on instantiation
    }
    
    // Interface
    public void Collect()
    {
        PlayerWallet moneyScript = FindObjectOfType<PlayerWallet>();
        moneyScript.IncreaseBioCompound(bioGranted);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
            CoinManager.DeregisterCoin(this);
            Destroy(gameObject);
        }
    }
}
