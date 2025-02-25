using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterBaseStatsData 
{
    // make base stats for player
    //make base buff
    //make amount of stats increase per level: for example 1000HP -> 1100HP
    public float PlayerHealth;
    public int PlayerDamage;
    public float PlayerSpeed;
    public float AttackSpeed;
    public float PlayerRecovery;
    public int PlayerDashCharge;
    public int PlayerDashRecovery;
    public int PlayerUpgradePoint;
    public int PlayerCurrentLevel;
    public int PlayerMaxLevel;

    public float HealthBuff;
    public float SpeedBuff;
    public float DamageBuff;
    public float AttackSpeedBuff;

    public int HealthUpgradeAmount;
    public int SpeedUpgradeAmount;
    public int StrengthUpgradeAmount;
    public int RecoveryUpgradeAmount;
    public int DashChargeUpgradeAmount;
    public int DashRecoveryUpgradeAmount;

}

public enum UpgradeType // make upgrade type
{
    Damage,
    DashRecovery,
    DashCharge,
    MovementSpeed,
    Health,
    Recovery,
    AttackSpeed
}

public enum BuffType // make buff type
{
    Health,
    Damage,
    MovementSpeed,
    FConversionRate,
    AttackSpeed
}

