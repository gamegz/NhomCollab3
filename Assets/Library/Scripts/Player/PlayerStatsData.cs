using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatsData 
{
    public CharacterStatsData currentCharacterStats;

    public void Init(CharacterBaseStatsData baseStats)
    {
        currentCharacterStats = new CharacterStatsData();
        SetBaseStats(baseStats);
    }
    public CharacterStatsData GetCharacterStats
    {
        get { return currentCharacterStats; }
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
