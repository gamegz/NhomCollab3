using System;
using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class PlayerBase : MonoBehaviour, IDamageable // THIS SCRIPT WILL HANDLE THE PLAYER'S RELATIONSHIP WITH OTHER SCRIPT TO PREVENT CREATING UNNECESSARY SCRIPTS [DUC ANH]
{
    
    public static PlayerBase Instance { get ; private set; }

    PlayerInput _playerInput;
    [SerializeField] private LayerMask interactLayerMask;
    private Transform _playerTransform;
    private Rigidbody rb;
    [HideInInspector] public float currentPlayerHealth;
    // System Action Stuff
    public delegate void OnHealthModified(float modifiedHealth, float maxHealth, bool? increased);
    public static event OnHealthModified HealthModified;    
    public delegate void OnHealBarFull(bool isOverHealing);
    public static event OnHealBarFull HBOverheal;

    #region Animation Related Stuff
    public delegate void OnHealReady(bool isReady, string displayText);
    public static event OnHealReady HealReady;    
    public delegate void OnHealActivated();
    public static event OnHealActivated HealActivated;
    private Coroutine switchReadyTextCoroutine = null;
    #endregion
    
    // for UI
    // clamped value, should update bar via tween
    public delegate void OnOverHealChange(float currentClampedValue, float maxValue, bool shouldUpdateViaTween);
    public static event OnOverHealChange OnOverHealValueChange; 

    #region Heal Bar & Over Heal Stuff
    public enum HealthStates
    {
        INCREASED,
        DECREASED
    }
    private Dictionary<HealthStates, bool> healthStatesDictionary;
    private Coroutine overHealCoroutine = null;
    private bool isOverHealing = false;
    private bool overHealReady = false;
    private float overHealTimer = 0f; 
    private float currentHBProgress = 0f; 
    private float clampedHBValue = 0f; 
    [SerializeField] private float HBIncreaseAmount = 1f; 
    [SerializeField] private float maxHBRequirement = 6f;
    #endregion

    #region Health Stuff
    //[Tooltip("Health Update Level <=> Amount of health added to player's highest current health")]
    //[SerializeField] private float healthUpgradeAmount = 1f; // Will be used when the upgrade system is established
    //[SerializeField] private float maxHealth;
    [Tooltip("The amount of time player became immune to damage after being hit")]
    [SerializeField] private float invulTimeAfterDamaged = 0.5f;
    private float invulTimeAfterDamagedCount;
    private bool canBeDamage = true;
    private Coroutine damageImmuneCoroutine;
    public bool CanBeDamage => canBeDamage;

    #endregion

    #region Interaction and InteractionUI
    private IInteractable currentInteractable;

    private bool isHolding;
    [SerializeField] private float holdTime;
    [SerializeField] private float holdCounter = 0f;
    //ref
    private PlayerUI playerUI;
    #endregion
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

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
        //PlayerDatas.Instance.GetStats.currentPlayerHealth = PlayerDatas.Instance.GetStats.healthStat;
        HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, PlayerDatas.Instance.GetStats.healthStat, SetHealthState(HealthStates.INCREASED));
        playerUI = GetComponent<PlayerUI>();
        invulTimeAfterDamagedCount = invulTimeAfterDamaged;
        OnOverHealValueChange?.Invoke(clampedHBValue, maxHBRequirement, false);
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

    //private void OnTriggerSpeedBuff()
    //{
    //    buffSpeed = 0.2f;
    //}

    //private void EndBuffDuration()
    //{
    //    buffSpeed = 1f;
    //}

    private void OnInteractWithObject(InputAction.CallbackContext context)
    {
        Debug.LogWarning($"Interact pressed. Current Interactable: {currentInteractable}");

        if (currentInteractable == null)
        {
            Debug.LogWarning("No interactable object found!");
            //return;
        }

        GameObject interactableObject = (currentInteractable as MonoBehaviour)?.gameObject;
        if (context.performed)
        {
            if (interactableObject != null && GameManager.Instance.isRespawnPointClaimed(interactableObject))
            {
                Debug.Log("Opening Respawn Selection UI...");
                GameManager.Instance.EnterOverviewMode();
                //return;
            }

            Debug.Log("asdasdasd");
            isHolding = true;
            holdCounter = 0f;
            StartCoroutine(HoldToClaim());
        }

        if(context.canceled)
        {
            isHolding = false;
            holdCounter = 0f;
        }
        
    }

    public void OnUpgradeCharacter()
    {
        //PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.MovementSpeed, 1);
        //PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.Health, 1);
        //PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.Recovery, 1);
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
        //Debug.Log("Player Health: " + PlayerDatas.Instance.GetStats.currentPlayerHealth);
    }

    private IEnumerator HoldToClaim()
    {
        while(isHolding && holdCounter < holdTime)
        {
            holdCounter += Time.deltaTime;
            yield return null;
        }

        if(holdCounter >= holdTime)
        {
            currentInteractable.OnInteract();
        }
        playerUI.ToggleInstructionText(false);
        playerUI.ToggleInstructionText2(true);
        isHolding = false;
    }

    public void UpdatePlayerHealth()
    {
        HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, PlayerDatas.Instance.GetStats.healthStat, SetHealthState(HealthStates.INCREASED));
    }

    public void TakeDamage(int modifiedHealth) // ACTIVATED WHEN TAKING DAMAGE
    {
        if(!canBeDamage) { return; }

        if (isOverHealing)
        {
            isOverHealing = false;
            currentHBProgress = 0;
            clampedHBValue = 0;
        }
        else if (!isOverHealing)
        {
            if (currentHBProgress == maxHBRequirement && PlayerDatas.Instance.GetStats.currentPlayerHealth >= PlayerDatas.Instance.GetStats.healthStat - 1)
            {
                if (switchReadyTextCoroutine != null)
                {
                    StopCoroutine(switchReadyTextCoroutine);
                    switchReadyTextCoroutine = null;
                }

                if (currentHBProgress == maxHBRequirement && switchReadyTextCoroutine == null)
                    switchReadyTextCoroutine = StartCoroutine(SwitchTextReadyAnim());
                
                else
                    HealReady?.Invoke(false, "[  Attack To Enter Over Heal  ]"); 

                overHealReady = false;
            }
            
            //PlayerDatas.Instance.OnPlayerHealthChange(modifiedHealth);
            PlayerDatas.Instance.GetStats.currentPlayerHealth -= modifiedHealth;

            HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, PlayerDatas.Instance.GetStats.healthStat, SetHealthState(HealthStates.DECREASED));
        }

        if (PlayerDatas.Instance.GetStats.currentPlayerHealth <= 0)
        {
            OnPlayerDeath();
        }
        
        OnOverHealValueChange?.Invoke(clampedHBValue, maxHBRequirement, true);
        damageImmuneCoroutine = StartCoroutine(CanDamageStatusCountDown(invulTimeAfterDamaged));
    }

    public void StartImmunityCoroutine(float immunityTime)
    {
        if(damageImmuneCoroutine == null)
        {
            damageImmuneCoroutine = StartCoroutine(CanDamageStatusCountDown(immunityTime));
        }
    }
    private IEnumerator CanDamageStatusCountDown(float invulTime)
    {
        canBeDamage = false;
        invulTimeAfterDamagedCount = invulTime;
        //yield return new WaitForSeconds(invulTime);
        while(invulTimeAfterDamagedCount > 0)
        {
            invulTimeAfterDamagedCount -= Time.deltaTime;
            yield return null;
        }
        canBeDamage = true;
    }

    private IEnumerator SwitchTextReadyAnim()
    {
        HealReady?.Invoke(false, "[  Attack To Enter Over Heal  ]");
        yield return new WaitForSeconds(0.5f);
        HealReady?.Invoke(true, "[  Charged Attack To Heal  ]");

        switchReadyTextCoroutine = null;
    }

    public void IncreaseHealBar(bool byChargedAttack) // ACTIVATED WHEN HITTING AN ENEMY
    {
        currentHBProgress = Mathf.Clamp(currentHBProgress + HBIncreaseAmount, 0, maxHBRequirement + 1);
        clampedHBValue = Mathf.Clamp(currentHBProgress, 0, maxHBRequirement);

        if (clampedHBValue == maxHBRequirement)
        {
            if (PlayerDatas.Instance.GetStats.currentPlayerHealth == PlayerDatas.Instance.GetStats.healthStat && !overHealReady)
            {
                overHealReady = true;
                HealReady?.Invoke(true, "[  Attack To Enter Over Heal  ]"); 
            }
            else if (PlayerDatas.Instance.GetStats.currentPlayerHealth < PlayerDatas.Instance.GetStats.healthStat)
            {
                HealReady?.Invoke(true, "[  Charged Attack To Heal  ]");
            }

            if (PlayerDatas.Instance.GetStats.currentPlayerHealth < PlayerDatas.Instance.GetStats.healthStat && byChargedAttack)
            {
                currentHBProgress = 0;
                PlayerDatas.Instance.GetStats.currentPlayerHealth++;

                HealActivated?.Invoke();
                HealthModified?.Invoke(PlayerDatas.Instance.GetStats.currentPlayerHealth, PlayerDatas.Instance.GetStats.healthStat, SetHealthState(HealthStates.INCREASED));
            }
            else if (currentHBProgress > maxHBRequirement && PlayerDatas.Instance.GetStats.currentPlayerHealth == PlayerDatas.Instance.GetStats.healthStat && overHealCoroutine == null)
            { 
                isOverHealing = true;
                overHealCoroutine = StartCoroutine(OverHealing());
                HealActivated?.Invoke();
                HBOverheal?.Invoke(true);
            }
        }
        
        var value = Mathf.Clamp(currentHBProgress, 0, maxHBRequirement);
        OnOverHealValueChange?.Invoke(value, maxHBRequirement, true);
    }

    private IEnumerator OverHealing()
    {
        while (clampedHBValue >= 0)
        {
            overHealTimer += Time.deltaTime;

            if (HBIncreaseAmount < 3) HBIncreaseAmount += overHealTimer / 25000;
            else HBIncreaseAmount = 3;

            clampedHBValue -= overHealTimer / 1250f;
            currentHBProgress = clampedHBValue;
            OnOverHealValueChange?.Invoke(clampedHBValue, maxHBRequirement,false);
            yield return null;            
        }

        overHealReady = false;
        isOverHealing = false;
        HBOverheal?.Invoke(false);
        currentHBProgress = 0;
        clampedHBValue = 0; 
        overHealTimer = 0;
        HBIncreaseAmount = 1;

        overHealCoroutine = null;
    }


    public float GetHealBarProgress() // For displaying heal bar UI 
    {
        return Mathf.InverseLerp(0f, maxHBRequirement, currentHBProgress); 
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
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        GameManager.Instance.UpdateGameState(GameState.LOSE);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitForSceneLoad());
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitUntil(() => GameManager.Instance.GetSpawnPoint() != null);
        yield return new WaitForSeconds(0.5f);
        SetPlayerPosition();
    }

    private void SetPlayerPosition()
    {
        Transform spawnPoint = GameManager.Instance.GetSpawnPoint();
        if (spawnPoint != null)
        {
            rb.position = spawnPoint.position;
            PlayerDatas.Instance.LoadGame();
            UpdatePlayerHealth();
        }
        else
        {
            //Debug.Log("haha");
        }
        
    }

    public void Teleport(Vector3 position, Quaternion targetRotation)
    {
        if(rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        rb.position = position;

        if (rb != null)
        {
            rb.detectCollisions = true;
            rb.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            GameManager.Instance.SetCurrentRespawnPoint(other.gameObject);
            Debug.Log($"Interactable object detected: {other.gameObject.name}");
            if (GameManager.Instance.isRespawnPointClaimed(other.gameObject))
            {
                playerUI.ToggleInstructionText2(true);
                
                return;
            }
            playerUI.ToggleInstructionText(true);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<IInteractable>() == currentInteractable)
        {
            playerUI.ToggleInstructionText(false);
            playerUI.ToggleInstructionText2(false);
            GameManager.Instance.SetCurrentRespawnPoint(null);
            currentInteractable = null;
        }
    }
}
