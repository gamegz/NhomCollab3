using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEconomy : MonoBehaviour
{
    private int bioCompound;
    private int credits;

    private int silver; //???????

    [SerializeField] private float pullSpeed = 3f;
    [SerializeField] private float bioCompoundPerCredit = 3f;

    void Start()
    {
       
    }

    void Update()
    {
        PullCoins();

        if (Input.GetKeyDown(KeyCode.T))
        {
            ExchangeBioCompoundForCredit();
        }
    }

    // Gain money
    public void IncreaseBioCompound(int amount)
    {
        bioCompound += amount;
        Debug.Log(bioCompound);
    }


    // Pull all Coins in Scene
    private void PullCoins()
    {
        // Get all registered coins from the CoinManager
        List<Coin> coins = CoinManager.GetCoins();

        foreach (Coin coin in coins)
        {
            // Move the coin towards the player
            coin.transform.position = Vector3.MoveTowards(coin.transform.position, transform.position, pullSpeed * Time.deltaTime);
        }
    }

    private void ExchangeBioCompoundForCredit()
    {
        int creditGain = Mathf.CeilToInt(bioCompound / bioCompoundPerCredit);

        // Update credits and reset BioCompound count to 0 after exchange
        credits += creditGain;
        bioCompound = 0;

        Debug.Log($"Exchanged BioCompound for Credits. Total Credits: {credits}");
    }

}
