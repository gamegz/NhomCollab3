using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Object", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    public PlayerStatsStruct baseStats;
}
