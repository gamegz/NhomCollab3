using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
//Keep track of all player base stats
//Stats are float, convert when use by other script
public class CharacterBaseStatsData 
{
    //Player Base stats - use for calculation or stat reset
    public float PlayerHealth;
    public int PlayerDamage;
    public float PlayerMoveSpeed;
    public float PlayerAttackSpeed;
    public float PlayerRecovery;
    public int PlayerDashCharge;
    public int PlayerDashRecovery;
    

    public int PlayerGemCount; //Number of gem
    public int PlayerMaxLevel;
    public int PlayerCurrentLevel;

    public float CurrentExperienceAmount;
    public float maxExperienceAmount;
    public float overflowExperienceAmount;

    //Buff - NOTE: WILL NOT BE USE
    public float HealthBuff;
    public float SpeedBuff;
    public float DamageBuff;
    public float AttackSpeedBuff;

    ////Amount of percent to add when upgrade - unchange in gameplay
    //public int HealthUpgradeAmount;
    //public int SpeedUpgradeAmount;
    //public int StrengthUpgradeAmount;
    //public int RecoveryUpgradeAmount;
    //public int DashChargeUpgradeAmount;
    //public int DashRecoveryUpgradeAmount;

    ////Max level of stats - unchange in gameplay
    //public int HealthMaxLevel;
    //public int SpeedMaxLevel;
    //public int StrengMaxLevel;
    //public int RecoveryMaxLevel;
    //public int DashChargeMaxLevel;
    //public int DashRecoveryMaxLevel;

    //Current Level of stats

       
}



public enum UpgradeType // make upgrade type
{
    Damage,
    DashRecovery,
    DashCharge,
    MovementSpeed,
    Health,
    Recovery,
    //AttackSpeed
}

public enum BuffType // make buff type
{
    Health,
    Damage,
    MovementSpeed,
    FConversionRate,
    AttackSpeed
}

