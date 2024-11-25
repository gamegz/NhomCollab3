using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatsData 
{
    public CharacterStatsData currentCharacterStats;

    public void Init()
    {
        CharacterStatsData statsData = new CharacterStatsData();
    }
    public CharacterStatsData GetCharacterStats
    {
        get { return currentCharacterStats; }
    }

    public void SetBaseStats(CharacterBaseStatsData characterStats)
    {
        GetCharacterStats.SetBaseStat(characterStats);
    }
}
