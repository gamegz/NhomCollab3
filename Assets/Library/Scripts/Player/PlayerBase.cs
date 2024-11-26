using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBase : MonoBehaviour, IDamageable
{
    public PlayerBattleData data;
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
        //MeshRenderer childMeshRender = GetComponentInChildren<MeshRenderer>();
        //_playerTransform = childMeshRender ? childMeshRender.transform : null;
        _playerTransform = GetComponent<Transform>();
        if( _playerInput != null)
        {
            Debug.Log("its work");
        }

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

    public float MoveSpeed
    {
        get
        {
            float modifier = buffSpeed + (SPEED_INCREASE_PER_LEVEL * moveSpeedLevel);
            return data.MoveSpeed(modifier);
        }
    }

    public float Health
    {
        get
        {
            float modifier = buffHealth + (HEALTH_INCREASE_PER_LEVEL * healthLevel);
            return data.Health(modifier);
        }
    }

    public float FConversionRate
    {
        get
        {
            float modifier = buffFConversionRate + (FCONVERSION_RATE_INCREASE_PER_LEVEL * fConversionRateLevel);
            return data.FConversionRate(modifier);
        }
    }

    public int Damage
    {
        get
        {
            int modifier = buffDamage + (DAMAGE_INCREASE_PER_LEVEL * damageLevel);
            return data.Damage(modifier);
        }
    }

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
        Debug.Log("isITWork????");
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Bao's Scene");
            Debug.Log("Heyyyyy");
        }
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
            Debug.Log("haha");
        }
    }
}
