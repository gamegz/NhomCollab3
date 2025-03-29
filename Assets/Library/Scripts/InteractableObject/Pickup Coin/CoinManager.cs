using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{

    // Create a list that contains all instantiated coins in the scene
    private static List<Coin> coins = new List<Coin>();

    // Add Coins
    public static void RegisterCoin(Coin coin)
    {
        if (!coins.Contains(coin))
        {
            coins.Add(coin);
        }
    }

    // Remove Coins
    public static void DeregisterCoin(Coin coin)
    {
        if (coins.Contains(coin))
        {
            coins.Remove(coin);
        }
    }

    // Get Coin
    public static List<Coin> GetCoins()
    {
        return coins;
    }
}