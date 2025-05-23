using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro; // Testing

public class PlayerWallet : MonoBehaviour
{
    private int bioCompound;
    private int credit;

    [SerializeField] private float pullSpeed = 3f;
    [SerializeField] private float bioCompoundPerCredit = 3f;

    // Testing
    [SerializeField] private TextMeshPro moneyText;

    public static PlayerWallet P_WalletInstance { get; private set; }

    private void Awake()
    {
        if (P_WalletInstance == null)
        {
            P_WalletInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
       
    }

    void Update()
    {
        PullCoins();

        // Testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            ExchangeBioCompoundForCredit();
        }
        if (Input.GetKeyDown(KeyCode.C))  
        {
            AddMoney();
        }
        PrintMoney(); 
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

        // Update credits and reset BioCompound count to 0
        credit += creditGain;
        bioCompound = 0;
    }

    public bool DeductBioCompound(int amount)
    {
        if (bioCompound >= amount)
        {
            bioCompound -= amount;
            return true;  
        }
        else
        {
            Debug.LogWarning("Not enough bioCompound!");
            return false; 
        }
    }

    public bool DeductCredit(int amount)
    {
        if (credit >= amount)
        {
            credit -= amount;
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough Credit!");
            return false;
        }
    }


    // Testing
    private void PrintMoney()
    {
        moneyText.text = "BioCompound: " + bioCompound + "\nCredit: " + credit;
    }

    private void AddMoney()
    {
        bioCompound += 500;
        credit += 500;
    }
}
