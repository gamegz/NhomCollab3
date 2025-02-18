using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterBaseStatsData 
{
    public float PlayerHealth;
    public int PlayerDamage;
    public float PlayerSpeed;
    public float FConversionRate;
    public int PlayerUpgradePoint;
    public float HealthBuff;
    public float SpeedBuff;
    public float DamageBuff;
    public float AttackSpeedBuff;
    public float AttackSpeed;
    public float HealthIncreasePerLevel;
    public float SpeedIncreasePerLevel;
    public float FConversionRateIncreasePerLevel;
    public float AttakSpeedIncreasePerLevel;
    public int DamageIncreasePerLevel;

}

public enum UpgradeType
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

public enum BuffType
{
    Health,
    Damage,
    MovementSpeed,
    FConversionRate,
    AttackSpeed
}

