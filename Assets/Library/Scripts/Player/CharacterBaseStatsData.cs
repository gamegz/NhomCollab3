using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterBaseStatsData 
{
    //Player Base stats
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

    //Buff - NOTE: WILL NOT BE USE
    public float HealthBuff;
    public float SpeedBuff;
    public float DamageBuff;
    public float AttackSpeedBuff;

    //Amount of percent to add when upgrade - unchange in gameplay
    public int HealthUpgradeAmount;
    public int SpeedUpgradeAmount;
    public int StrengthUpgradeAmount;
    public int RecoveryUpgradeAmount;
    public int DashChargeUpgradeAmount;
    public int DashRecoveryUpgradeAmount;

    ////Max level of stats - unchange in gameplay
    //public int HealthMaxLevel;
    //public int SpeedMaxLevel;
    //public int StrengMaxLevel;
    //public int RecoveryMaxLevel;
    //public int DashChargeMaxLevel;
    //public int DashRecoveryMaxLevel;

    //Current Level of stats
    public int HealthCurrentLevel;
    public int SpeedCurrentLevel;
    public int StrengthCurrentLevel;
    public int RecoveryCurrentLevel;
    public int DashChargeCurrentLevel;
    public int DashRecoveryCurrentLevel;   
}



public enum UpgradeType // make upgrade type
{
    Strength,
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

