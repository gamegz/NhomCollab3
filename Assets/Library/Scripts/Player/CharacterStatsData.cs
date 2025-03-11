using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterStatsData
{
    
    public float healthStat;
    public float damageStat;
    public float moveSpeedStat;
    public float attackSpeedStat;
    public float recoveryStat;
    public float dashChargeStat;
    public float dashRecoveryStat;

    public int HealthCurrentLevel;
    public int SpeedCurrentLevel;
    public int StrengthCurrentLevel;
    public int RecoveryCurrentLevel;
    public int DashChargeCurrentLevel;
    public int DashRecoveryCurrentLevel;

    public Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();

    private CharacterBaseStatsData baseStats;
    public float currentPlayerHealth;

    public void SetBaseStat(CharacterBaseStatsData baseStats)
    {
        this.baseStats = baseStats;

        healthStat = baseStats.PlayerHealth;
        damageStat = baseStats.PlayerDamage;
        moveSpeedStat = baseStats.PlayerMoveSpeed;
        recoveryStat = baseStats.PlayerRecovery;
        dashChargeStat = baseStats.PlayerDashCharge;
        dashRecoveryStat = baseStats.PlayerDashRecovery;

        currentPlayerHealth = healthStat;  
    }

    //Change the value directly - called by StatsUpgrade script
    public void OnStatsUpgrade(UpgradeType upgradeType, float value, int level)
    {
        switch (upgradeType)
        {
            case UpgradeType.Health:
                healthStat = value;
                HealthCurrentLevel = level;
                break;
            case UpgradeType.MovementSpeed:
                moveSpeedStat = value;
                SpeedCurrentLevel = level;
                break;
            case UpgradeType.Recovery:
                recoveryStat = value;
                RecoveryCurrentLevel = level;
                break;
            case UpgradeType.Damage:
                damageStat = value;
                StrengthCurrentLevel = level;
                break;
            case UpgradeType.DashCharge:
                dashChargeStat = value;
                DashChargeCurrentLevel = level;
                break;
            case UpgradeType.DashRecovery:
                dashRecoveryStat = value;
                DashRecoveryCurrentLevel = level;
                break;
            default:
                Debug.LogWarning("Unknown UpgradeType: " + upgradeType);
                break;
        }
    }

    public void ReassignBaseStats(CharacterBaseStatsData baseStats)
    {
        this.baseStats = baseStats; // Just assign the reference, do NOT reset upgraded stats
    }

    //public void OnTriggerBuff(BuffType buffType, float BuffPower)
    //{
    //    BuffTypes[buffType] = BuffPower;
    //}

    //public void OnEndBuff(BuffType buffType)
    //{
    //    BuffTypes[buffType] = 1;
    //}

    //public void OnTriggerDamageBuff(BuffType buffType, int BuffPower)
    //{
    //    DamageBuff[buffType] = BuffPower;
    //}

    //public void OnEndDamageBuff(BuffType buffType)
    //{
    //    DamageBuff[buffType] = 1;
    //}

    public void OnPlayerHealthChange(float value)
    { 
        currentPlayerHealth -= value;
    }


    public float BaseHealth => baseStats.PlayerHealth;
    public float BaseMoveSpeed => baseStats.PlayerMoveSpeed;
    public float BaseRecovery => baseStats.PlayerRecovery;
    //public float BaseAttackSpeed => baseStats.PlayerAttackSpeed;
    //Player only hold 1 weapon
    public int BaseDamage => baseStats.PlayerDamage;
    public int BaseMaxDashCharge => baseStats.PlayerDashCharge;
    public float BaseDashRecovery => baseStats.PlayerDashRecovery;
    


    //True stats after upgrade - implement these stats
    public float Health => healthStat;
    public float MoveSpeed => moveSpeedStat;
    public float Recovery => recoveryStat;
    public float AttackSpeed => attackSpeedStat;
    //Player only hold 1 weapon
    public float Damage => damageStat;
    public float MaxDashCharge => dashChargeStat;
    public float DashRecovery => dashRecoveryStat;
}
