using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public int money;
    public int Money { get; set; }
}

public class User
{
    private int money;
    public int Money => money;
    public int Money2 { get; private set; }

    public void ModifyMoney(int money)
    {
        this.money += money;
        Money2 += money;
    }
}

public class Thief
{
    public void Execute()
    {
        BankAccount account = new BankAccount();
        account.OnDeposit(2);
        Debug.Log(account.Money.ToString());
    }
}

public class BankAccount
{
    //private int money;
    public int Money { get; private set; }

    [SerializeField] private int moneyAccount;
    public int MoneyAccount => moneyAccount;
    public int GetMoneyAccount()
    {
        return moneyAccount;
    }


    #region modify money value
    public void OnDeposit(int money)
    {
        this.Money += money;
    }

    public void OnWithdraw(int money)
    {
        this.Money -= money;
    }
    #endregion
}

public class Player
{
    public static int playerCount;

}

public class LobbyManager
{
    public void PlayerJoinLobby(Player player)
    {
        if (Player.playerCount > 4)
            return;

        Player.playerCount++;
    }

    public void ExitLobby(Player player)
    {
        Player.playerCount--;
    }
}



public interface testInterface
{
    public abstract void OnInteract();
}

public abstract class Parent : testInterface
{
    public virtual void test() { Debug.Log("Parent"); }
    public abstract void abstractTest();

    public void OnInteract()
    {
        throw new System.NotImplementedException();
    }
}

public class Child : Parent
{
    public override void test() { }

    public override void abstractTest() { }
}