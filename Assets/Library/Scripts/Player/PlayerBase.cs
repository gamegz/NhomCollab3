using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBase : MonoBehaviour, IDamageable // THIS SCRIPT WILL HANDLE THE PLAYER'S RELATIONSHIP WITH OTHER SCRIPT TO PREVENT CREATING UNNECESSARY SCRIPTS [DUC ANH]
{
    
    public static PlayerBase Instance { get ; private set; }

    // public PlayerBattleData data;
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

    // System Action Stuff
    public delegate void OnHealthModified(float modifiedHealth, bool? increased);
    public static event OnHealthModified HealthModified;    
    public delegate void OnHealBarFull();
    public static event OnHealBarFull HBFull;

    // Variable for Heal Bar, which is associated with hearts and Overheal
    public enum HealthStates
    {
        INCREASED,
        DECREASED
    }
    private Dictionary<HealthStates, bool> healthStatesDictionary;
    private int currentHBProgress = 0;
    private int maxHBProgress = 3;

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

        // For Invoking Stuff
        InitializeHealthStatesDictionary();
    }

    private void Start()
    {
        HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, SetHealthState(HealthStates.INCREASED));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _playerInput.Player.OnInteract.performed += OnInteractWithObject;
        _playerInput.Player.OnInteract.canceled += OnInteractWithObject;
        _playerInput.Enable();

        EnemyBase.OnEnemyDamaged += ModifyHealBar;        
    }

    private void OnDisable()
    {
        WeaponManager.CurrentWeapon -= OnSaveWeaponPrefab;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _playerInput.Player.OnInteract.performed -= OnInteractWithObject;
        _playerInput.Player.OnInteract.canceled -= OnInteractWithObject;
        _playerInput.Disable();
        
        EnemyBase.OnEnemyDamaged -= ModifyHealBar;
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
        PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.FConversionRate, 1);

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

    public void TakeDamage(int modifiedHealth)
    {
        //Debug.Log("Damage Taken: " + modifiedHealth);

        PlayerDatas.Instance.OnPlayerHealthChange(modifiedHealth);

        //Debug.Log(PlayerDatas.Instance.GetStats.currentPlayerHealth);

        HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, SetHealthState(HealthStates.DECREASED));

        if (PlayerDatas.Instance.GetStats.currentPlayerHealth <= 0)
        {
            OnPlayerDeath();
        }
    }

    private void ModifyHealBar() // Either increase or reset the heal bar of the player
    {
        currentHBProgress++;
        if (currentHBProgress >= maxHBProgress)
        {
            if (PlayerDatas.Instance.GetStats.currentPlayerHealth < 5)
            {
                PlayerDatas.Instance.GetStats.currentPlayerHealth++;
                HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, SetHealthState(HealthStates.INCREASED));
            }
            HBFull?.Invoke();
            currentHBProgress = 0;
        }

        Debug.Log(currentHBProgress);
    }

    public int GetHealBarProgress()
    {
        return 0; // Change this later
    }

    private void InitializeHealthStatesDictionary()
    {
        healthStatesDictionary = new Dictionary<HealthStates, bool>
        {
            { HealthStates.INCREASED, true },
            { HealthStates.DECREASED, false } 
        };
    }

    private bool? SetHealthState(HealthStates state)
    {
        Debug.Log("Set Health State: " + state);
        return healthStatesDictionary.ContainsKey(state) ? healthStatesDictionary[state] : (bool?)null; 
    }



    private void OnPlayerDeath()
    {
        GameManager.Instance.UpdateGameState(GameState.LOSE);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
