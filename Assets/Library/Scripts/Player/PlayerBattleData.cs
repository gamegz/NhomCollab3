using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerBattleData
{
    public PlayerData playerData;

    public float Health(float modifier)
    {
        return playerData.baseStats.Health * modifier;
    }

    public float MoveSpeed(float modifier)
    {
        return playerData.baseStats.movementSpeed * modifier;
    }

    public float FConversionRate(float modifier)
    {
        return playerData.baseStats.fConversionRate * modifier;
    }

    public int Damage(int modifier)
    {
        return playerData.baseStats.Damage * modifier;
    }
}

