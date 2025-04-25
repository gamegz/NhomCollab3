using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action OnPlayerDeathEvent;
    //this class to do bunch of stuff but the important part for stats is in Awake method
    float previousTimeScale = 1f;
    bool isPaused;
    public bool isPlayerDead = false;
    public Transform SpawnPoint;
    public Transform nextSpawnPoint;
    public List <GameObject> RespawnPoint = new List <GameObject>();

    [SerializeField] private string mainMenuName = "MainMenu";
    [SerializeField] private string playRoomName = "AssetFillMain";
    [HideInInspector] public GameState state;
    [HideInInspector] public bool inOverviewMode = false;
    private GameObject currentRespawnPoint;
    public static GameManager Instance { get; private set; }

    public static event Action<GameState> OnGameStateChange;
    private void Awake()
    {
        Cursor.visible = true;

        PlayerDatas.Instance.LoadGame();
        if(PlayerDatas.Instance.GetStats.currentPlayerHealth <= 0)
        {
            PlayerDatas.Instance.GetStats.currentPlayerHealth = PlayerDatas.Instance.GetStats.Health;
            PlayerDatas.Instance.SaveGame();
        }
        //if (Instance == null)
        //{
        //    Instance = this;
        //}

        DontDestroyOnLoad(this.gameObject);
        #region Singleton
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        #endregion

        //Testing
        state = GameState.SELECTGAME;
    }

    [SerializeField] private GameObject pausePanel;

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (newState)
        {
            
            case GameState.SELECTGAME:
                Time.timeScale = 1;
                SceneManager.LoadScene(mainMenuName);
                //Save Game
                break;
            case GameState.OPENMENU:
                //Cursor.visible = true;
                Time.timeScale = 0;
                //Pause Game
                break;
            case GameState.PLAYING:
                //Cursor.visible = false;
                SceneManager.LoadScene(playRoomName);
                Time.timeScale = 1;
                break;
            case GameState.WIN:
                //Cursor.visible = true;
                break;
            case GameState.LOSE:
                //Cursor.visible = true;
                isPlayerDead = true;
                OnPlayerDeathEvent?.Invoke();
                TogglePause();
                break;
        }

        OnGameStateChange?.Invoke(newState);
    }

    public void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            isPaused = true;
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = previousTimeScale;
            isPaused = false;
        }
    }

    public void PublicTogglePause()
    {
        TogglePause();
        if (Time.timeScale == 1f && pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public Transform GetSpawnPoint()
    {
        if (RespawnPoint.Count > 0)
        {
            return RespawnPoint[RespawnPoint.Count - 1].transform; // Last claimed respawn point
        }
        return SpawnPoint;
    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }

    public bool isRespawnPointClaimed(GameObject respawnPoint)
    {
        return RespawnPoint.Contains(respawnPoint);
    }

    public void ClaimRespawnPoimt(GameObject respawnPoimt)
    {
        if (!RespawnPoint.Contains(respawnPoimt))
        {
            RespawnPoint.Add(respawnPoimt);
        }
    }

    public List<GameObject> GetClaimedRespawnPoints()
    {
        
        Debug.Log("Claimed Respawn Points: " + RespawnPoint.Count);
        foreach (var point in RespawnPoint)
        {
            Debug.Log("Respawn Point: " + point.name);
        }
        return RespawnPoint;
    }

    public void SetCurrentRespawnPoint(GameObject respawnPoint)
    {
        currentRespawnPoint = respawnPoint;
        //Debug.Log(currentRespawnPoint.name);
    }

    public GameObject GetCurrentRespawnPoint()
    {
        return currentRespawnPoint;
    }

    public void EnterOverviewMode()
    {
        if(inOverviewMode) return;
        inOverviewMode = true;
        UIManager.Instance.ShowRespawnSelectionUI();
    }

    public void ExitOverviewMode()
    {
        inOverviewMode = false;
    }

    public void TeleportPlayerToRespawnPoint(GameObject targetPoint)
    {
        if (targetPoint == currentRespawnPoint) return;
        
        Vector3 teleportPosition = targetPoint.transform.position + new Vector3(0, 1f, 0); 
        PlayerBase.Instance.Teleport(teleportPosition, targetPoint.transform.rotation);
        SectionReset sectionReset = targetPoint.GetComponent<SectionReset>();
        if (sectionReset != null)
        {
            sectionReset.ResetRoomAfterTeleport();
        }
        ExitOverviewMode();
        GameManager.Instance.isPlayerDead = false;
    }

    private void OnApplicationQuit()
    {
        PlayerDatas.Instance.SaveGame();
    }

    public void PublicOnApplicationQuit()
    {
        OnApplicationQuit();
        #if UNITY_STANDALONE
        Application.Quit();
        #endif
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}

public enum GameState
{
    SELECTGAME,
    OPENMENU,
    PLAYING, 
    LOSE,
    WIN,
    
    HOMELOBBY,    
    MENU
}
