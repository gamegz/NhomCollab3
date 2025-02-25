using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBase : MonoBehaviour, IDamageable
{
    // THIS MUST BE A SINGLETON CLASS RIGHT?
    public static PlayerBase Instance { get ; private set; }

    //public PlayerBattleData data;
    private int moveSpeedLevel;
    private int healthLevel;
    private int fConversionRateLevel;
    private int damageLevel;
    private const float SPEED_INCREASE_PER_LEVEL = 0.02f;
    private const float HEALTH_INCREASE_PER_LEVEL = 1f;
    private const float FCONVERSION_RATE_INCREASE_PER_LEVEL = 1f;
    private const int DAMAGE_INCREASE_PER_LEVEL = 1;
    private float buffSpeed = 1f;
    private float buffHealth = 1f;
    private float buffFConversionRate = 1f;
    private int buffDamage = 1;
    PlayerInput _playerInput;
    [SerializeField] private LayerMask interactLayerMask;
    private Transform _playerTransform;

    private void Awake()
    {
         _playerInput = new PlayerInput();
        _playerTransform = GetComponent<Transform>();

        for(int i = 0; i < Object.FindObjectsOfType<PlayerBase>().Length; i++)
        {
            if (Object.FindObjectsOfType<PlayerBase>()[i] != this )
            {
                if (Object.FindObjectsOfType<PlayerBase>()[i].name == gameObject.name)
                {
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }
        }
        WeaponManager.CurrentWeapon += OnSaveWeaponPrefab;
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _playerInput.Player.OnInteract.performed += OnInteractWithObject;
        _playerInput.Player.OnInteract.canceled += OnInteractWithObject;
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        WeaponManager.CurrentWeapon -= OnSaveWeaponPrefab;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _playerInput.Player.OnInteract.performed -= OnInteractWithObject;
        _playerInput.Player.OnInteract.canceled -= OnInteractWithObject;
        _playerInput.Disable();
    }

    private void OnTriggerSpeedBuff()
    {
        buffSpeed = 0.2f;
    }

    private void EndBuffDuration()
    {
        buffSpeed = 1f;
    }

    //public float MoveSpeed
    //{
    //    get
    //    {
    //        float modifier = buffSpeed + (SPEED_INCREASE_PER_LEVEL * moveSpeedLevel);
    //        return data.MoveSpeed(modifier);
    //    }
    //}

    //public float Health
    //{
    //    get
    //    {
    //        float modifier = buffHealth + (HEALTH_INCREASE_PER_LEVEL * healthLevel);
    //        return data.Health(modifier);
    //    }
    //}

    //public float FConversionRate
    //{
    //    get
    //    {
    //        float modifier = buffFConversionRate + (FCONVERSION_RATE_INCREASE_PER_LEVEL * fConversionRateLevel);
    //        return data.FConversionRate(modifier);
    //    }
    //}

    //public int Damage
    //{
    //    get
    //    {
    //        int modifier = buffDamage + (DAMAGE_INCREASE_PER_LEVEL * damageLevel);
    //        return data.Damage(modifier);
    //    }
    //}


    private void OnInteractWithObject(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Collider[] colliders = Physics.OverlapSphere(_playerTransform.position, 0.5f, interactLayerMask);
            foreach (Collider collide in colliders)
            {
                IInteractable interactable = collide.GetComponent<IInteractable>();
                interactable?.OnInteract();
            }
        }
        
    }

    public void OnUpgradeCharacter()
    {
        PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.MovementSpeed, 1);
        PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.Health, 1);
        PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.Recovery, 1);

    }


    private void OnSaveWeaponPrefab()
    {
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
    }

    private void OnDrawGizmos()
    {
        if(_playerTransform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_playerTransform.position, 0.5f);
        }
        
    }
    private void Update()
    {

    }

    public void TakeDamage(int damageAmount)
    {
        PlayerDatas.Instance.OnPlayerHealthChange(damageAmount);
        Debug.Log(PlayerDatas.Instance.GetStats.currentPlayerHealth);
        if (PlayerDatas.Instance.GetStats.currentPlayerHealth <= 0)
        {
            OnPlayerDeath();
        }
    }
    private void OnPlayerDeath()
    {
        GameManager.Instance.UpdateGameState(GameState.LOSE);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitForSceneLoad());
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(0.05f);
        SetPlayerPosition();
    }

    private void SetPlayerPosition()
    {
        Transform spawnPoint = GameManager.Instance.GetSpawnPoint();
        if(spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }
        else
        {
            //Debug.Log("haha");
        }
    }
}
