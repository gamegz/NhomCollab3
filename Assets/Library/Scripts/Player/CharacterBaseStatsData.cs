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

    public float FConversionRate;
    public int PlayerUpgradePoint;
    public float HealthBuff;
    public float SpeedBuff;
    public float DamageBuff;
    public float AttackSpeedBuff;

    public float HealthIncreasePerLevel;
    public float SpeedIncreasePerLevel;
    public float FConversionRateIncreasePerLevel;
    public float AttakSpeedIncreasePerLevel;
    public int DamageIncreasePerLevel;

}

public enum UpgradeType // make upgrade type
{
    Damage,
    DashRecovery,
    DashCharge,
    MovementSpeed,
    Health,
    FConversionRate,
    AttackSpeed,
    StaggerTime
}

public enum BuffType // make buff type
{
    Health,
    Damage,
    MovementSpeed,
    FConversionRate,
    AttackSpeed
}

