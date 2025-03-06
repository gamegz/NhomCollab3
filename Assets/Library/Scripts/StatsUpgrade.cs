using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class keep track of the upgrade stats

//When upgrade, the ugrade UI script calls this class Functions
public class StatsUpgrade : MonoBehaviour
{
    Dictionary<UpgradeType, UpgradeGroup> upgradeGroupDic = new Dictionary<UpgradeType, UpgradeGroup>();
    public Dictionary<UpgradeType, UpgradeGroup> UpgradeGroupDic => upgradeGroupDic;
    public UpgradeGroup healthUpgradeGroup;
    public UpgradeGroup speedUpgradeGroup;
    public UpgradeGroup strengthUpgradeGroup;
    public UpgradeGroup recoveryUpgradeGroup;
    public UpgradeGroup dashChargeUpgradeGroup;
    public UpgradeGroup dashRecoveryUpgradeGroup;
    [Space]

    private int gemCount = 1000;
    public int GemCount { get { return gemCount; } set { gemCount = value; } }

    private void Start()
    {

        upgradeGroupDic = new Dictionary<UpgradeType, UpgradeGroup>
        {
            {UpgradeType.Health, healthUpgradeGroup},
            {UpgradeType.MovementSpeed, speedUpgradeGroup},
            {UpgradeType.Strength, strengthUpgradeGroup},
            {UpgradeType.Recovery, recoveryUpgradeGroup},
            {UpgradeType.DashCharge, dashChargeUpgradeGroup},
            {UpgradeType.DashRecovery, dashRecoveryUpgradeGroup},
        };
    }

    public void UpgradeStats(UpgradeType upgradeType, int upgradeNum)
    {
        UpgradeGroup upgradeTarget = upgradeGroupDic[upgradeType];
        upgradeTarget.currentLevel += upgradeNum;
        upgradeTarget.trueStats = GetCurrentStats(
            upgradeTarget.baseStat,
            upgradeTarget.upgradePercentIncrease,
            upgradeTarget.currentLevel,
            upgradeNum);

        upgradeGroupDic[upgradeType] = upgradeTarget;
    }

    public int GetCurrentStats(int baseStat, int upgradePercentIncrease, int currentLevel, int upgradeNum)
    {
        float percentIncrease = 100 + (upgradePercentIncrease * (upgradeNum + currentLevel));
        return  (int)((baseStat / 100) * percentIncrease); 
    }

    //Temp for UI
    public int GetPercentIncrease(UpgradeType upgradeType, int upgradeNum)
    {
        int increaseAmount = upgradeGroupDic[upgradeType].upgradePercentIncrease;
        int currentLevel = upgradeGroupDic[upgradeType].currentLevel;

        return 100 + (increaseAmount * (upgradeNum + currentLevel));
    }
}

[System.Serializable]
//Only change currentLevel and trueStats in gameplay
public struct UpgradeGroup
{
    public int baseStat;
    public int upgradePercentIncrease;
    public int upgradeRequirement;
    public int currentLevel;
    public int maxLevel;
    public int trueStats;

    public UpgradeGroup(int baseStat, int upgradePercentIncrease, int upgradeRequirement, int currentLevel, int maxLevel, int trueStats)
    {
        this.baseStat = baseStat;
        this.upgradePercentIncrease = upgradePercentIncrease;
        this.upgradeRequirement = upgradeRequirement;
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
        this.trueStats = trueStats;
    }
}