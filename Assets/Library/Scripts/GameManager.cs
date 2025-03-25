using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //this class to do bunch of stuff but the important part for stats is in Awake method
    float previousTimeScale = 1f;
    bool isPaused;
    public Transform SpawnPoint;
    public List <GameObject> RespawnPoint = new List <GameObject>();
    [HideInInspector] public GameState state;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        PlayerDatas.Instance.LoadGame();
        PlayerDatas.Instance.GetStats.currentPlayerHealth = PlayerDatas.Instance.GetStats.Health;
        //Rigidbody rb = PlayerBase.Instance.GetComponent<Rigidbody>();
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [SerializeField] private GameObject pausePanel;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePause();
            if (Time.timeScale == 0f && pausePanel != null) 
            {
                pausePanel.SetActive(true);
            }
            else if (Time.timeScale == 1f && pausePanel != null)
            {
                pausePanel.SetActive(false);

            }
        }
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (newState)
        {
            case GameState.LOSE:
                TogglePause();
                UIManager.Instance.OnEnableLosePanel();
                break;
            case GameState.HOMELOBBY:
                SceneManager.LoadScene("HomeRoomScene");
                TogglePause();
                break;
        }
    }

    private void TogglePause()
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
        Transform spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
        if (spawnPoint != null)
        {
            return spawnPoint;
        }
        else
        {
            return null;
        }
    }

    public void ChangeScene(string sceneName)
    {
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
    LOSE
}
