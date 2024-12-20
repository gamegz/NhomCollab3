using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterStatsData
{
    public Dictionary<UpgradeType, int> upgradeLevel = new Dictionary<UpgradeType, int>()
    {
        {UpgradeType.MovementSpeed, 0 },
        {UpgradeType.Health, 0 },
        {UpgradeType.FConversionRate, 0 },
        {UpgradeType.AttackSpeed, 0 },
        {UpgradeType.Damage, 0},
        //{UpgradeType.StaggerTime, 1f}
    };

    public Dictionary<BuffType, float> BuffTypes = new Dictionary<BuffType, float>()
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

    public float GetFConversionRate(float modifier)
    {
        return baseStats.FConversionRate * modifier;
    }

    public float GetAttackSpeed(float modifier)
    {
        return baseStats.AttackSpeed * modifier;
    }

    public float Health
    {
        get
        {
            float modifier = BuffTypes[BuffType.Health] + (baseStats.HealthIncreasePerLevel * upgradeLevel[UpgradeType.Health]);
            return GetHealth(modifier);
        }
    }
    public float MoveSpeed
    {
        get
        {
            float modifier = BuffTypes[BuffType.MovementSpeed] + (baseStats.SpeedIncreasePerLevel * upgradeLevel[UpgradeType.MovementSpeed]);
            return GetMovementSpeed(modifier);
        }
    }
    public float FConversionRate
    {
        get
        {
            float modifier = BuffTypes[BuffType.FConversionRate] + (baseStats.FConversionRateIncreasePerLevel * upgradeLevel[UpgradeType.FConversionRate]);
            return GetFConversionRate(modifier);
        }
    }

    public float AttackSpeed
    {
        get
        {
            float modifier = BuffTypes[BuffType.AttackSpeed] + (baseStats.AttakSpeedIncreasePerLevel * upgradeLevel[UpgradeType.AttackSpeed]);
            return GetAttackSpeed(modifier);
        }
    }

    public int DamageModifier
    {
        get
        {
            int modifier = DamageBuff[BuffType.Damage] + (baseStats.DamageIncreasePerLevel * upgradeLevel[UpgradeType.Damage]);
            return GetDamage(modifier);
        }
    }
}
