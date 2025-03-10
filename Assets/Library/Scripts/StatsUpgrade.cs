using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class keep track of the Upgrade stats and Upgrade fomula
//Upgrade function will change the base stats
//Sending value to feed to Upgrade menu UI
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
        SetUpUpgradeGroup();
        
    }

    private void SetUpUpgradeGroup()
    {
        //Assign base stats
        healthUpgradeGroup.baseStat = PlayerDatas.Instance.GetStats.BaseHealth;
        speedUpgradeGroup.baseStat = PlayerDatas.Instance.GetStats.BaseMoveSpeed;
        strengthUpgradeGroup.baseStat = PlayerDatas.Instance.GetStats.BaseDamage;
        recoveryUpgradeGroup.baseStat = PlayerDatas.Instance.GetStats.BaseRecovery;
        dashChargeUpgradeGroup.baseStat = PlayerDatas.Instance.GetStats.BaseMaxDashCharge;
        dashRecoveryUpgradeGroup.baseStat = PlayerDatas.Instance.GetStats.BaseDashRecovery;
        Debug.Log(speedUpgradeGroup.baseStat);
        //Create dic
        upgradeGroupDic = new Dictionary<UpgradeType, UpgradeGroup>
        {
            {UpgradeType.Health, healthUpgradeGroup},
            {UpgradeType.MovementSpeed, speedUpgradeGroup},
            {UpgradeType.Damage, strengthUpgradeGroup},
            {UpgradeType.Recovery, recoveryUpgradeGroup},
            {UpgradeType.DashCharge, dashChargeUpgradeGroup},
            {UpgradeType.DashRecovery, dashRecoveryUpgradeGroup},
        };
    }


    //Call from the UpgradeUI - CONSUME button
    public void UpgradeStats(UpgradeType upgradeType, int upgradeNum)
    {  
        UpgradeGroup upgradeTarget = upgradeGroupDic[upgradeType];
        upgradeTarget.currentLevel += upgradeNum;
        upgradeTarget.trueStats = GetCurrentStats(
        upgradeTarget.baseStat,
        upgradeTarget.upgradePercentIncrease,
        upgradeTarget.currentLevel,
        upgradeNum);
        //Replace the stats
        PlayerDatas.Instance.OnStatsUpgrade(upgradeType, upgradeTarget.trueStats);
        upgradeGroupDic[upgradeType] = upgradeTarget;
    }

    public float GetCurrentStats(float baseStat, int upgradePercentIncrease, int currentLevel, int upgradeNum)
    {
        float percentIncrease = 100 + (upgradePercentIncrease * (upgradeNum + currentLevel));
        return  ((baseStat / 100) * percentIncrease); 
    }

    //Temp for UI stats text
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
    [HideInInspector]
    public float baseStat;
    public int upgradePercentIncrease;
    public int upgradeRequirement;
    [HideInInspector]
    public int currentLevel;
    public int maxLevel;
    [HideInInspector]
    public float trueStats;

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