using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterStatsData
{
    // make dictionary of the upgradeLevel of each stats, pls make it a integer
    public Dictionary<UpgradeType, int> upgradeLevel = new Dictionary<UpgradeType, int>()
    {
        {UpgradeType.MovementSpeed, 0 },
        {UpgradeType.Health, 0 },
        {UpgradeType.Recovery, 0 },
        {UpgradeType.AttackSpeed, 0 },
        {UpgradeType.Damage, 0},
        {UpgradeType.DashCharge, 0},
        {UpgradeType.DashRecovery, 0}
        //{UpgradeType.StaggerTime, 1f}
    };
    public struct UpgradeStats
    {
        public int baseStats;
        public int upgradeAmount;
        public int upgradeRequirement;
        public int maxUpgradeLevel;
        public int currentUpgradeLevel;

        public UpgradeStats(int baseStats, int upgradeAmount, int upgradeRequirement, int maxUpgradeLevel, int currentUpgradeLevel)
        {
            this.baseStats = baseStats;
            this.upgradeAmount = upgradeAmount;
            this.upgradeRequirement = upgradeRequirement;
            this.maxUpgradeLevel = maxUpgradeLevel;
            this.currentUpgradeLevel = currentUpgradeLevel;
        }
    }

    public UpgradeStats speedUpgradeStats;
    public UpgradeStats healthUpgradeStats;
    public UpgradeStats recoveryUpgradeStats;
    public UpgradeStats damageUpgradeStats;
    public UpgradeStats dashChargeUpgradeStats;
    public UpgradeStats dashRecoveryUpgradeStats;


    //public Dictionary<UpgradeType, UpgradeStats> upgradeGroupDic = new Dictionary<UpgradeType, UpgradeStats>()
    //{
    //    {UpgradeType.MovementSpeed, speedUpgradeStats}
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

    public void OnStatsUpgrade(UpgradeType upgradeType, int value)
    {
        upgradeLevel[upgradeType] += value;
        if(upgradeType == UpgradeType.Health)
        {
            currentPlayerHealth = Health;
        }
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

    // make a get set function

    public float GetHealth(float modifier)
    {
        return baseStats.PlayerHealth * modifier;
    }

    public int GetDamage(int modifier)
    {
        return baseStats.PlayerDamage * modifier;
    }

    public float GetMovementSpeed(float modifier)
    {
        return baseStats.PlayerSpeed * modifier;
    }

    public float GetRecovery(float modifier)
    {
        return baseStats.PlayerRecovery * modifier;
    }

    public float GetAttackSpeed(float modifier)
    {
        return baseStats.AttackSpeed * modifier;
    }

    // same for down here, follow the equation
    public float Health
    {
        get
        {
            float modifier = (100 + upgradeLevel[UpgradeType.Health]) * 0.01f;
            return GetHealth(modifier);
        }
    }
    public float MoveSpeed
    {
        get
        {
            float modifier = (100 + upgradeLevel[UpgradeType.MovementSpeed]) * 0.01f;
            return GetMovementSpeed(modifier);
        }
    }
    public float Recovery
    {
        get
        {
            float modifier = (100 + upgradeLevel[UpgradeType.Recovery]) * 0.01f;
            return GetRecovery(modifier);
        }
    }

    public float AttackSpeed
    {
        get
        {
            float modifier = (100 + upgradeLevel[UpgradeType.Health]) * 0.01f;
            return GetAttackSpeed(modifier);
        }
    }

    public int DamageModifier
    {
        get
        {
            int modifier = (int)((100 + upgradeLevel[UpgradeType.Health]) * 0.01f);
            return GetDamage(modifier);
        }
    }
}
