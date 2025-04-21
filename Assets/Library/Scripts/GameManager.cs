using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [HideInInspector] public GameState state;
    [SerializeField] private Camera playerCamera;
    //[SerializeField] private Camera overviewCamera;
    [HideInInspector] public bool inOverviewMode = false;
    private GameObject currentRespawnPoint;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Debug.Log(Time.timeScale);

        PlayerDatas.Instance.LoadGame();
        if(PlayerDatas.Instance.GetStats.currentPlayerHealth <= 0)
        {
            PlayerDatas.Instance.GetStats.currentPlayerHealth = PlayerDatas.Instance.GetStats.Health;
            PlayerDatas.Instance.SaveGame();
        }
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [SerializeField] private GameObject pausePanel;

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Escape))
        //{
        //    TogglePause();
        //    if (Time.timeScale == 0f && pausePanel != null)
        //    {
        //        pausePanel.SetActive(true);
        //    }
        //    else if (Time.timeScale == 1f && pausePanel != null)
        //    {
        //        pausePanel.SetActive(false);

        //    }
        //}
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (newState)
        {
            case GameState.LOSE:
                isPlayerDead = true;
                OnPlayerDeathEvent?.Invoke();
                TogglePause();
                //UIManager.Instance.OnEnableLosePanel();
                break;
            case GameState.HOMELOBBY:
                SceneManager.LoadScene("HomeRoomScene");
                TogglePause();
                break;
            case GameState.MENU:
                SceneManager.LoadScene("MainMenu");
                break;
        }
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
        Application.Quit();
    }

}

public enum GameState
{
    HOMELOBBY,
    LOSE,
    MENU
}
