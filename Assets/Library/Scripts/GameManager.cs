using System;
using System.Collections;
using System.Collections.Generic;
using Library.Scripts.Audio;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List <GameObject> RespawnPoint = new List <GameObject>();
    public Transform SpawnPoint;
    public bool isPlayerDead = false;

    [SerializeField] private GameState StartingGameState = GameState.SELECTGAME; //For testing
    [SerializeField] private string mainMenuName = "MainMenu";
    [SerializeField] private string playRoomName = "AssetFillMain";
    [HideInInspector] public GameState state;
    [HideInInspector] public bool inOverviewMode = false; 
    public SO_SoundData soundData;
    private AudioManager _audioManager;
    private GameObject currentRespawnPoint;
    public static GameManager Instance { get; private set; }

    public static event Action<GameState> OnGameStateChange;
    public static event Action OnPlayerDeathEvent;
    
    [Header("GlobalSound")]
    public AudioSource townSound;
    public AudioSource bossBattleMusic;
    private void Awake()
    {
        Cursor.visible = true;
        _audioManager = AudioManager.Instance;
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
            Destroy(this.gameObject);
        }
        #endregion

        ////Testing
        //state = GameState.SELECTGAME;
    }

    [SerializeField] private GameObject pausePanel;

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (newState)
        {
            
            case GameState.SELECTGAME:
                Time.timeScale = 1;
                if (RespawnPoint.Count > 0)
                {
                    RespawnPoint.Clear();
                }
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
                //SceneManager.LoadScene(playRoomName);
                Time.timeScale = 1;
                break;
            case GameState.WIN:
                //Cursor.visible = true;
                break;
            case GameState.LOSE:
                //Cursor.visible = true;
                isPlayerDead = true;
                OnPlayerDeathEvent?.Invoke();
                //UIManager.Instance.OnEnableLosePanel();
                //TogglePause();
                Time.timeScale = 0;
                break;
        }

        OnGameStateChange?.Invoke(newState);
    }
    
    public void OnEnterRoom()
    {
        townSound?.Stop();
    }

    public void OnEnterBossRoom()
    {
        townSound?.Stop();
        bossBattleMusic?.Play();
    }

    public void OnFinishBossRoom()
    {
        bossBattleMusic?.Stop();
        townSound?.Play();
    }

    public void OnExitRoom()
    {
        townSound?.Play();
    }

    public Transform GetSpawnPoint()
    {
        if (RespawnPoint == null)
        {
            if (SpawnPoint == null)
            {
                SpawnPoint = GameObject.Find("SpawnPoint").transform;
            }
        }
        if (RespawnPoint.Count > 0)
        {
            return RespawnPoint[RespawnPoint.Count - 1].transform; // Last claimed respawn point
        }

        if (SpawnPoint == null)
        {
            GameObject spawnObject = GameObject.Find("SpawnPoint");
            if (spawnObject != null)
            {
                SpawnPoint = spawnObject.transform;
            }
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
        
        foreach (var point in RespawnPoint)
        {
            //Debug.Log("Respawn Point: " + point.name);
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
        //UIManager.Instance.ShowRespawnSelectionUI();
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

    public void PlaySound(Sound sound, float volume = 1)
    {
        int randomIndex = 0;
        switch (sound)
        {
            case Sound.dash:
                _audioManager.PlaySoundEffect(soundData.DashSound).SetSourceVolume(volume);
                break;
            case Sound.attack:
                randomIndex = Random.Range(0, soundData.attackSound.Count);
                _audioManager.PlaySoundEffect(soundData.attackSound[randomIndex]).SetSourceVolume(volume);
                break;
            case Sound.chargeAttack:
                randomIndex = Random.Range(0, soundData.chargeAttackSound.Count);
                _audioManager.PlaySoundEffect(soundData.chargeAttackSound[randomIndex]).SetSourceVolume(volume);
                break;
            case Sound.parryInnit:
                _audioManager.PlaySoundEffect(soundData.parryInnit).SetSourceVolume(volume);
                break;
            case Sound.parrySuccess:
                randomIndex = Random.Range(0, soundData.parrySuccess.Count);
                _audioManager.PlaySoundEffect(soundData.parrySuccess[randomIndex]).SetSourceVolume(volume);
                break;
            case Sound.playerHurt:
                _audioManager.PlaySoundEffect(soundData.PlayerHurtSound).SetSourceVolume(volume);
                break;
            case Sound.playerHeal:
                _audioManager.PlaySoundEffect(soundData.PlayerHealSound).SetSourceVolume(volume);
                break;
            case Sound.pickUpSound:
                randomIndex = Random.Range(0, soundData.PickupSound.Count);
                _audioManager.PlaySoundEffect(soundData.PickupSound[randomIndex]).SetSourceVolume(volume);
                break;
            case Sound.levelUpSound:
                _audioManager.PlaySoundEffect(soundData.LevelUpSound).SetSourceVolume(volume);
                break;
            case Sound.enemyAttackIndicator:
                _audioManager.PlaySoundEffect(soundData.AttackIndicatorSound).SetSourceVolume(volume);
                break;
            case Sound.enemyFootStep:
                _audioManager.PlaySoundEffect(soundData.FootStepSound).SetSourceVolume(volume);
                break;
            case Sound.enemyHurt:
                randomIndex = Random.Range(0, soundData.hurtSound.Count);
                _audioManager.PlaySoundEffect(soundData.hurtSound[randomIndex]).SetSourceVolume(volume);
                break;
            case Sound.zombieSound:
                randomIndex = Random.Range(0, soundData.zombieSound.Count);
                _audioManager.PlaySoundEffect(soundData.zombieSound[randomIndex]).SetSourceVolume(volume);
                break;
        }
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

public enum Sound
{
    footstep,
    dash,
    attack,
    chargeAttack,
    parryInnit,
    parrySuccess,
    playerHurt,
    playerHeal,
    pickUpSound,
    levelUpSound,
    enemyAttackIndicator,
    enemyFootStep,
    enemyHurt,
    zombieSound
}
