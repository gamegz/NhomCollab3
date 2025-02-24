using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
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
    public delegate void OnHealBarFull(bool isOverHealing);
    public static event OnHealBarFull HBOverheal;

    #region Animation Activation
    public delegate void OnHealReady(bool isReady, string displayText);
    public static event OnHealReady HealReady;    
    public delegate void OnHealActivated();
    public static event OnHealActivated HealActivated;
    #endregion

    // Variable for Heal Bar, which is associated with hearts and Overheal
    public enum HealthStates
    {
        INCREASED,
        DECREASED
    }
    private Dictionary<HealthStates, bool> healthStatesDictionary;
    private Coroutine overHealCoroutine;
    private bool isOverHealing = false;
    private bool overHealReady = false;
    private float overHealTimer = 0f; 
    private float currentHBProgress = 0f; 
    private float clampedHBValue = 0f; 
    private float HBMultiplier = 1f; 
    private float maxHBProgress = 6f;

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

        EnemyBase.OnEnemyDamaged += IncreaseHealBar;        
    }

    private void OnDisable()
    {
        WeaponManager.CurrentWeapon -= OnSaveWeaponPrefab;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _playerInput.Player.OnInteract.performed -= OnInteractWithObject;
        _playerInput.Player.OnInteract.canceled -= OnInteractWithObject;
        _playerInput.Disable();
        
        EnemyBase.OnEnemyDamaged -= IncreaseHealBar;
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
        Debug.Log("Player Health: " + PlayerDatas.Instance.GetStats.currentPlayerHealth);

    }

    public void TakeDamage(int modifiedHealth) // ACTIVATED WHEN TAKING DAMAGE
    {
        if (isOverHealing)
        {
            HBOverheal?.Invoke(false);
            isOverHealing = false;
            currentHBProgress = 0;
            clampedHBValue = 0;
        }
        else if (!isOverHealing)
        {
            if (currentHBProgress == maxHBProgress && PlayerDatas.Instance.GetStats.currentPlayerHealth >= 4)
            {
                overHealReady = false;
                HealReady?.Invoke(false, "[  Over Heal Ready  ]"); 
            }
            
            PlayerDatas.Instance.OnPlayerHealthChange(modifiedHealth);

            HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, SetHealthState(HealthStates.DECREASED));


        }

        if (PlayerDatas.Instance.GetStats.currentPlayerHealth <= 0)
        {
            OnPlayerDeath();
        }
    }

    public void IncreaseHealBar(bool byChargedAttack) // ACTIVATED WHEN HITTING AN ENEMY
    {
        currentHBProgress = Mathf.Clamp(currentHBProgress + HBMultiplier, 0, maxHBProgress + 1);
        clampedHBValue = Mathf.Clamp(currentHBProgress, 0, maxHBProgress);

        if (clampedHBValue == maxHBProgress)
        {
            if (PlayerDatas.Instance.GetStats.currentPlayerHealth == 5 && !overHealReady)
            {
                overHealReady = true;
                HealReady?.Invoke(true, "[  Attack To Enter Over Heal  ]"); 
            }
            else if (PlayerDatas.Instance.GetStats.currentPlayerHealth < 5)
            {
                HealReady?.Invoke(true, "[  Charged Attack To Heal  ]");
            }

            if (PlayerDatas.Instance.GetStats.currentPlayerHealth < 5 && byChargedAttack)
            {
                currentHBProgress = 0;
                PlayerDatas.Instance.GetStats.currentPlayerHealth++;

                HealActivated?.Invoke();
                HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, SetHealthState(HealthStates.INCREASED));
            }
            else if (currentHBProgress > maxHBProgress && PlayerDatas.Instance.GetStats.currentPlayerHealth == 5 && overHealCoroutine == null)
            { 
                isOverHealing = true;
                overHealCoroutine = StartCoroutine(OverHealing());
                HealActivated?.Invoke();
                HBOverheal?.Invoke(true);
            }
        }

    }

    private IEnumerator OverHealing()
    {
        while (clampedHBValue >= 0)
        {
            overHealTimer += Time.deltaTime;

            if (HBMultiplier < 3)
                HBMultiplier += overHealTimer / 25000;
            
            else
                HBMultiplier = 3;

            clampedHBValue -= overHealTimer / 1250f;
            currentHBProgress = clampedHBValue;
            yield return null;            
        }

        overHealReady = false;
        isOverHealing = false;
        HBOverheal?.Invoke(false);
        currentHBProgress = 0;
        clampedHBValue = 0; 
        overHealTimer = 0;
        HBMultiplier = 1;

        overHealCoroutine = null;
    }


    public float GetHealBarProgress() // For displaying heal bar UI 
    {
        return Mathf.InverseLerp(0f, maxHBProgress, currentHBProgress); 
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
