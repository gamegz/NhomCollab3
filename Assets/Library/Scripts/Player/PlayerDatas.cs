using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PlayerDatas
{
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
        LoadBaseStats();
        LoadPlayerStats();
        playerStatsData.SetBaseStats(baseStatsData);
        
    }

    private void SaveGame()
    {
        string json = JsonConvert.SerializeObject(playerStatsData, Formatting.Indented);
        File.WriteAllText(saveFilePath, json);
        Debug.LogWarning("Game Save: " + json);
    }

    private void LoadBaseStats()
    {
        TextAsset baseStatsTextAssets = Resources.Load<TextAsset>("PlayerBaseStats");
        baseStatsData = JsonConvert.DeserializeObject<CharacterBaseStatsData>(baseStatsTextAssets.text);
    }

    private void LoadPlayerStats()
    {
        if(File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerStatsData = JsonConvert.DeserializeObject<PlayerStatsData>(json);
            Debug.Log("Player Stats Loaded: " + json);
            Debug.LogWarning(Application.persistentDataPath);
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

    public void OnStatsUpgrade(UpgradeType upgradeType, int value)
    {
        playerStatsData.GetCharacterStats.OnStatsUpgrade(upgradeType, value);
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
