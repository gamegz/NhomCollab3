using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public PlayerStatsData playerStatsData;
    public CharacterBaseStatsData baseStatsData = new CharacterBaseStatsData();

    public void LoadGame()
    {
        
    }

    private void LoadBaseStats()
    {
        TextAsset baseStatsTextAssets = Resources.Load<TextAsset>("PlayerBaseStats");
        //baseStatsData = JsonConvert.DeserializeObject<CharacterBaseStatsData>(baseStatsTextAssets.text);
    }

}
