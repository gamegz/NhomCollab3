using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float previousTimeScale = 1f;
    bool isPaused;
    public Transform SpawnPoint;
    [HideInInspector] public GameState state;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        PlayerDatas.Instance.LoadGame();
        PlayerDatas.Instance.GetStats.currentPlayerHealth = PlayerDatas.Instance.GetStats.Health;
        if(Instance == null)
        {
            Instance = this;
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
        if(Time.timeScale > 0)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            isPaused = true;
        }
        else if(Time.timeScale == 0)
        {
            Time.timeScale = previousTimeScale;
            isPaused = false;
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

}

public enum GameState
{
    HOMELOBBY,
    LOSE
}
