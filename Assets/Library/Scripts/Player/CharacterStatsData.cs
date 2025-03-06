using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterStatsData
{
    // make dictionary of the upgradeLevel of each stats, pls make it a integer
    public Dictionary<UpgradeType, float> upgradeLevel = new Dictionary<UpgradeType, float>()
    {
        {UpgradeType.MovementSpeed, 0 },
        {UpgradeType.Health, 0 },
        {UpgradeType.Recovery, 0 },
        {UpgradeType.AttackSpeed, 0 },
        {UpgradeType.Strength, 0},
        {UpgradeType.DashCharge, 0},
        {UpgradeType.DashRecovery, 0}
        //{UpgradeType.StaggerTime, 1f}
    };

    //public Dictionary<UpgradeType, int>  = new Dictionary<UpgradeType, int>()
    //{
    //    {UpgradeType.MovementSpeed, 0 },
    //    {UpgradeType.Health, 0 },
    //    {UpgradeType.Recovery, 0 },
    //    {UpgradeType.AttackSpeed, 0 },
    //    {UpgradeType.Damage, 0},
    //    {UpgradeType.DashCharge, 0},
    //    {UpgradeType.DashRecovery, 0}
    //    //{UpgradeType.StaggerTime, 1f}
    //};


    public Dictionary<BuffType, float> BuffTypes = new Dictionary<BuffType, float>() // this one also
    {
        {BuffType.Health, 1f},
        {BuffType.MovementSpeed, 1f },
        {BuffType.FConversionRate, 1f },
        {BuffType.AttackSpeed, 1f},
    };
    public Dictionary<BuffType, int> DamageBuff = new Dictionary<BuffType, int>()
    {
        {BuffType.Damage, 1},
    };

    private CharacterBaseStatsData baseStats;
    public float currentPlayerHealth;

    public void SetBaseStat(CharacterBaseStatsData baseStats)
    {
        this.baseStats = baseStats;
        currentPlayerHealth = Health;
    }

    //Change the value directly - called by StatsUpgrade script
    public void OnStatsUpgrade(UpgradeType upgradeType, float value)
    {
        upgradeLevel[upgradeType] += value;
        if(upgradeType == UpgradeType.Health)
        {
            currentPlayerHealth = Health;
        }
    }

    public void OnStatReset()
    {

    }

    public void OnTriggerBuff(BuffType buffType, float BuffPower)
    {
        BuffTypes[buffType] = BuffPower;
    }

    public void OnEndBuff(BuffType buffType)
    {
        BuffTypes[buffType] = 1;
    }

    public void OnTriggerDamageBuff(BuffType buffType, int BuffPower)
    {
        DamageBuff[buffType] = BuffPower;
    }

    public void OnEndDamageBuff(BuffType buffType)
    {
        DamageBuff[buffType] = 1;
    }

    public void OnPlayerHealthChange(float value)
    { 
        currentPlayerHealth -= value;
    }


    public float BaseHealth => baseStats.PlayerHealth;
    public float BaseMoveSpeed => baseStats.PlayerMoveSpeed;
    public float BaseRecovery => baseStats.PlayerRecovery;
    public float BaseAttackSpeed => baseStats.PlayerAttackSpeed;
    //Player only hold 1 weapon
    public int BaseDamage => baseStats.PlayerDamage;
    public int BaseMaxDashCharge => baseStats.PlayerDashCharge;
    public float BaseDashRecovery => baseStats.PlayerDashRecovery;
    


    //True stats after upgrade - implement these stats
    public float Health => baseStats.healthStat;
    public float MoveSpeed => baseStats.moveSpeedStat;
    public float Recovery => baseStats.recoveryStat;
    public float AttackSpeed => baseStats.attackSpeedStat;
    //Player only hold 1 weapon
    public int Damage => baseStats.damageStat;
    public int MaxDashCharge => baseStats.dashChargeStat;
    public float DashRecovery => baseStats.dashRecoveryStat;
}
