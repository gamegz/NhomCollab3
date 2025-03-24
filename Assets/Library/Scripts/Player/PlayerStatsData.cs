using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatsData 
{
    public CharacterStatsData currentCharacterStats;
    public Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();

    public void Init(CharacterBaseStatsData baseStats)
    {
        currentCharacterStats = new CharacterStatsData();
        SetBaseStats(baseStats);
    }
    public CharacterStatsData GetCharacterStats
    {
        get { return currentCharacterStats; }
    }

    public void SetUpgradeLevel(UpgradeType upgradeType, int level)
    {
        upgradeLevels[upgradeType] = level;
    }

    public int GetUpgradeLevel(UpgradeType upgradeType)
    {
        return upgradeLevels.ContainsKey(upgradeType) ? upgradeLevels[upgradeType] : 0;
    }

    public void SetBaseStats(CharacterBaseStatsData characterStats)
    {
        //GetCharacterStats.SetBaseStat(characterStats);
        if(currentCharacterStats == null)
        {
            currentCharacterStats = new CharacterStatsData();
        }
        currentCharacterStats.SetBaseStat(characterStats);
    }
}
