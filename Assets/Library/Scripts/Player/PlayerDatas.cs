using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PlayerDatas
{
    //save/load game
    private static PlayerDatas instance;
    public static PlayerDatas Instance
    {
        get
        {
            if (instance == null)
            {
                return instance = new PlayerDatas();
            }
            return instance;
        }
    }

    private string saveFilePath;
    public PlayerStatsData playerStatsData;
    public CharacterBaseStatsData baseStatsData = new CharacterBaseStatsData();

    public PlayerDatas()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerStats.json");
    }

    public void LoadGame()
    {
        //playerStatsData.SetBaseStats(baseStatsData);
        LoadBaseStats();
        LoadPlayerStats();
    }

    public void SaveGame()
    {
        string json = JsonConvert.SerializeObject(playerStatsData, Formatting.Indented);
        File.WriteAllText(saveFilePath, json);
        Debug.LogWarning("Game Save: " + json);
    }

    public void ReassignHealth()
    {
        playerStatsData.ReassignHealth();
    }

    private void LoadBaseStats()
    {
        TextAsset baseStatsTextAssets = Resources.Load<TextAsset>("PlayerBaseStats");
        baseStatsData = JsonConvert.DeserializeObject<CharacterBaseStatsData>(baseStatsTextAssets.text);
        Debug.Log("LoadBase");
    }

    private void LoadPlayerStats()
    {
        if(File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerStatsData = JsonConvert.DeserializeObject<PlayerStatsData>(json);
            Debug.Log("Player Stats Loaded: " + json);
            if (playerStatsData.GetCharacterStats != null)
            {
                playerStatsData.GetCharacterStats.ReassignBaseStats(baseStatsData);
            }
            Debug.LogWarning(Application.persistentDataPath);
            Debug.Log("LoadStats");
        }
        else
        {
            playerStatsData = new PlayerStatsData();
            CharacterBaseStatsData baseStats = baseStatsData;
            playerStatsData.Init(baseStatsData);
            playerStatsData.SetBaseStats(baseStatsData);
            SaveGame();
        }
    }


    public CharacterStatsData GetStats => playerStatsData?.GetCharacterStats;

    public void OnStatsUpgrade(UpgradeType upgradeType, float value, StatsUpgrade statsUpgrade)
    {
        int newLevel = playerStatsData.GetUpgradeLevel(upgradeType);
        playerStatsData.GetCharacterStats.OnStatsUpgrade(upgradeType, value, newLevel);
        playerStatsData.SetUpgradeLevel(upgradeType, newLevel);
        SaveGame();
        statsUpgrade?.LoadLevelFromData(playerStatsData.GetCharacterStats);
    }

    public void OnExperienceAndGemChange(float currentExperienceAmount, float maxExperienceAmount, int GemCount, StatsUpgrade statsUpgrade)
    {
        playerStatsData.GetCharacterStats.OnGemAndExperienceUpgrade(currentExperienceAmount, maxExperienceAmount, GemCount);
        SaveGame();
    }

    public void OnPlayerHealthChange(int Health)
    {
        playerStatsData.GetCharacterStats.OnPlayerHealthChange(Health);
    }

    public CharacterBaseStatsData GetBaseStats()
    {
        return baseStatsData;
    }

}
