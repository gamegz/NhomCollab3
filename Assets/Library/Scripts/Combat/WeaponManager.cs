using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private PlayerBase player;

    public delegate void CurrentWeaponHandler();
    public static event CurrentWeaponHandler CurrentWeapon;

    public delegate void OnWeaponChargingAttack(bool isHolding, float currentChargeTime, float maxChargeTime);
    public static event OnWeaponChargingAttack OnHoldChargeATK; // Duc Anh: THIS IS FOR UI.
    public static event Action<bool> OnPerformChargedATK;

    public delegate void OnWeaponCooldown(bool isCoolingDown, float currentRecoverTime, float maxRecoverTime);

    public static event OnWeaponCooldown OnCoolDownState;

    public delegate void OnHandlingAttack(int ComboCounter);
    public static event OnHandlingAttack AttackHandle;

    public delegate void handleMovementWhenRecover();
    public static event handleMovementWhenRecover HandleMovementWhenRecover;

    //Courotine
    Coroutine comboCoroutine;

    //Unity new Input system
    public PlayerInput _playerInput;

    //Weapon List attribute
    public List<WeaponBase> weaponList = new List<WeaponBase>();
    private WeaponBase _currentWeapon;
    private int _maxWeaponNum = 2;
    private int _weaponIndex = 0;

    //cooldown timer for normal attack and time between normal and charge attack
    private bool hasAttacked = false;
    private float cooldownTimer = 0f;
    private bool isDashingToCancelAction = false;
    private bool isAllowToCancelAction = false;

    //charge attack timer
    [SerializeField] private float _holdTime = 0f;
    private bool _isHoldAttack = false;
    private bool _isAttackInputting = false;
    private float comboAttackSpeed = 0.3f;

    /*IF you want to make the hold attacks. Try make a delay BEFORE starting the hold check
     This prevent player from registering normal attack as charge attack
     */
    [Header("ComboSystemAndAttack")]
    [SerializeField] int comboCounter = 0;
    int maxComboCount = 3;
    [Tooltip("Listen wait time for combo input")]
    [SerializeField] private float comboResetTime = 0.6f;
    [SerializeField] private float recoverDuration = 2f;
    float recoverTimer = 0f;
    public bool isRecovering = false;
    private bool _isAttacking;

    [Header("WeaponCollectRange")]
    [SerializeField] private float collectRange;

    private Transform playerTransform;
    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float _innitNormalToChargeAttackDelay; // This one
    //[SerializeField] private WeaponData swordData;
    //[SerializeField] private GameObject templateWeaponModel;
    //------------------------

    //Since each weapon have different delay time, need to overwrite the SO_WeaponData each time _currentWeapon value changes
    [Header("Animation")]
    [SerializeField] private PlayerAnimation playerAnimation;

    private Camera _camera; 

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _camera = Camera.main;
        playerTransform = transform.parent ?? transform;
        _currentWeapon = GetComponentInChildren<WeaponBase>();
        PlayerMovement.dashCancel += DashingToCancelAction;
        if (_currentWeapon != null)
        {
            _currentWeapon.GetComponent<BoxCollider>().enabled = false;
            if (weaponList.Count > 0)
            {
                Innit(weaponList[0]);
                _currentWeapon = weaponList[0];
            }
            else
            {
                Debug.Log("weaponList is empty");
            }
        }
    }

    private void OnDestroy()
    {
        PlayerMovement.dashCancel -= DashingToCancelAction;
    }

    private void Update()
    {
        if (_isAttackInputting && !isRecovering && !_isAttacking) // hold timer for charge attack
        {
            _holdTime += Time.deltaTime;
            if (_holdTime >= _currentWeapon._weaponData.holdThreshold) 
            {                                          
                _isHoldAttack = true;                                  
            }
            
            if (isDashingToCancelAction)
            {
                _isHoldAttack = false;
                _isAttackInputting = false;
                _holdTime = 0;
                isAllowToCancelAction = false;
            }

            if (_holdTime >= 0.2f)
            {
                OnHoldChargeATK?.Invoke(true, _holdTime, _currentWeapon._weaponData.holdThreshold);
            }
        }
        
        if (hasAttacked) // cooldown timer betweeen attack and between normal attack and charge attack
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                hasAttacked = false;
            }
        }
        
        if (isRecovering)
        {
            OnCoolDownState?.Invoke(true, recoverTimer, recoverDuration);
            
            if (recoverTimer <= 0f)
            {
                isRecovering = false;
                OnCoolDownState?.Invoke(false, recoverTimer, recoverDuration);
                return;
            }
            
            recoverTimer -= Time.deltaTime;
        }
    }

    public float GetHoldingChargeATKTime()
    {
        return _holdTime;
    }

    public float GetChargeATKProgress()
    {
        return _currentWeapon ? Mathf.InverseLerp(0f, _currentWeapon._weaponData.holdThreshold, _holdTime) : 0f;
    }

    public WeaponBase GetWeaponBaseRef()
    {
        return _currentWeapon;
    }

    void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.Player.Attack.performed += OnAttackInputPerform;
        _playerInput.Player.Attack.canceled += OnAttackInputEnd;
        _playerInput.Player.OnPickUpWeapon.performed += OnPickUpWeapon;
        _playerInput.Player.OnPickUpWeapon.canceled += OnPickUpWeapon;
        _playerInput.Player.OnSwitchWeapon.performed += OnSwitchWeapon;
        _playerInput.Player.OnSwitchWeapon.canceled += OnSwitchWeapon;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.Player.Attack.performed -= OnAttackInputPerform;
        _playerInput.Player.Attack.canceled -= OnAttackInputEnd;
        _playerInput.Player.OnPickUpWeapon.performed -= OnPickUpWeapon;
        _playerInput.Player.OnPickUpWeapon.canceled -= OnPickUpWeapon;
        _playerInput.Player.OnSwitchWeapon.performed -= OnSwitchWeapon;
        _playerInput.Player.OnSwitchWeapon.canceled -= OnSwitchWeapon;
    }

    private void Innit(WeaponBase StartingWeapon)
    {
        //Set current weapon
        //Enable Current weapon
        //if (_currentWeapon != null)
        //{
        //    _currentWeapon = StartingWeapon;
        //    _currentWeapon.GetComponent<BoxCollider>().enabled = false;
        //}
    }

    private void DashingToCancelAction()
    {
        if (isAllowToCancelAction)
        {
            isDashingToCancelAction = true;
            comboCounter = 0;
        }
    }

    private void OnAttackInputPerform(InputAction.CallbackContext context)
    {
        if (_isAttacking) return;
        
        if (_currentWeapon == null || isRecovering)
        {
            //Debug.Log("there is currently no weapon");
            return;
        }
        
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _isAttackInputting = true;
        }
        
        isAllowToCancelAction = true;
        _holdTime = 0f;
    }

    private void OnAttackInputEnd(InputAction.CallbackContext context) // this is Unity new input event for when the player release the mouse button
    {
        StartCoroutine(AttackRoutine());

        // if (EventSystem.current.IsPointerOverGameObject() && isRecovering)
        // {
        //     return; // Prevents player actions when clicking UI
        // }
        //
        // if (_currentWeapon != null)
        // {
        //     if (isDashingToCancelAction)
        //     {
        //         isDashingToCancelAction = false;
        //         return;
        //     }
        //
        //     if (_isHoldAttack)
        //     {
        //         Debug.Log("ChargeAttack");
        //         _currentWeapon.OnInnitSecondaryAttack();
        //         AttackHandle?.Invoke(comboCounter);
        //         cooldownTimer = _currentWeapon._weaponData.chargeAttackSpeed;
        //
        //         isAllowToCancelAction = false;
        //
        //         recoverTimer = recoverDuration;
        //         isRecovering = true;
        //         HandleMovementWhenRecover?.Invoke();
        //         OnPerformChargedATK?.Invoke(true);
        //
        //         _isAttackInputting = false;
        //         comboCounter = 0;
        //     }
        //     
        //     else if (!hasAttacked && !isRecovering) //if going for combo just focus on this statement
        //     {
        //         if (isRecovering) { return; }
        //         if (_holdTime >= 0.5f)
        //         {
        //             _holdTime = 0f;
        //             _isAttackInputting = false;
        //         }
        //
        //         _currentWeapon.OnInnitNormalAttack();
        //         if (comboCounter >= maxComboCount)
        //         {
        //             if (comboCoroutine != null)
        //             {
        //                 StopCoroutine(comboCoroutine);
        //             }
        //             
        //             comboCoroutine = StartCoroutine(ResetCombo());
        //         }
        //
        //         cooldownTimer = _currentWeapon._weaponData.attackSpeed;
        //         cooldownTimer = comboAttackSpeed;
        //         comboCounter++;
        //         playerAnimation.Attack(comboCounter);
        //         AttackHandle?.Invoke(comboCounter);
        //         OnPerformChargedATK?.Invoke(false);
        //         hasAttacked = true;
        //     }
        //
        //     //isAttack = true; // set the isAttack = true again so that it will start cooldown, avoid attack with no cooldown
        //     ResetAttackState();
        // }
        // //_isHoldAttack = true then charge attack and set the cooldown to weapon normal/charge attack speed and then start cooldown   
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        
        if (EventSystem.current.IsPointerOverGameObject() && isRecovering)
        {
            _isAttacking = false;
            yield break; // Prevents player actions when clicking UI
        }
        
        if (_currentWeapon != null)
        {
            if (isDashingToCancelAction)
            {
                isDashingToCancelAction = false;
                _isAttacking = false;
                yield break;
            }
            
            if (_camera)
            {
                var currentPos = playerTransform.position;
                var mousePos = Input.mousePosition;
                
                mousePos.z = (_camera.transform.position - currentPos).magnitude;
                var worldPos = _camera.ScreenToWorldPoint(mousePos);
                worldPos.y = currentPos.y;
                
                var targetDir = (worldPos - currentPos).normalized;

                var dot = Vector3.Dot(targetDir, transform.forward);
                while (dot < 0.98f)
                {
                    playerTransform.rotation = 
                        Quaternion.RotateTowards(playerTransform.rotation, Quaternion.LookRotation(targetDir, Vector3.up), 30);
                    yield return new WaitForFixedUpdate();
                    dot = Vector3.Dot(targetDir, transform.forward);
                }
            }

            if (_isHoldAttack)
            {
                Debug.Log("ChargeAttack");
                _currentWeapon.OnInnitSecondaryAttack();
                AttackHandle?.Invoke(comboCounter);
                cooldownTimer = _currentWeapon._weaponData.chargeAttackSpeed;
        
                isAllowToCancelAction = false;
        
                recoverTimer = recoverDuration;
                isRecovering = true;
                HandleMovementWhenRecover?.Invoke();
                OnPerformChargedATK?.Invoke(true);
        
                _isAttackInputting = false;
                comboCounter = 0;
            }
            
            else if (!hasAttacked && !isRecovering) //if going for combo just focus on this statement
            {
                if (isRecovering)
                {
                    _isAttacking = false;
                    yield break;
                }
                if (_holdTime >= 0.5f)
                {
                    _holdTime = 0f;
                    _isAttackInputting = false;
                }
        
                _currentWeapon.OnInnitNormalAttack();
                if (comboCounter >= maxComboCount)
                {
                    if (comboCoroutine != null)
                    {
                        StopCoroutine(comboCoroutine);
                    }
                    
                    comboCoroutine = StartCoroutine(ResetCombo());
                }
        
                cooldownTimer = _currentWeapon._weaponData.attackSpeed;
                cooldownTimer = comboAttackSpeed;
                comboCounter++;
                playerAnimation.Attack(comboCounter);
                AttackHandle?.Invoke(comboCounter);
                OnPerformChargedATK?.Invoke(false);
                hasAttacked = true;
            }
        
            //isAttack = true; // set the isAttack = true again so that it will start cooldown, avoid attack with no cooldown
            ResetAttackState();
            _isAttacking = false;
        }
        //_isHoldAttack = true then charge attack and set the cooldown to weapon normal/charge attack speed and then start cooldown   
    }
    
    private void ResetAttackState()
    {
        _holdTime = 0f;
        _isAttackInputting = false;
        _isHoldAttack = false;
        OnHoldChargeATK?.Invoke(false, 0, 0);
    }

    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(comboResetTime);

        isDashingToCancelAction = false;
        isAllowToCancelAction = false;

        recoverTimer = recoverDuration;
        isRecovering = true;

        //isAttack = true;
        cooldownTimer = _currentWeapon._weaponData.attackSpeed;
        comboCounter = 0;

        _isAttackInputting = false;
        _holdTime = 0f;
    }

    private IEnumerator ResetFullCombo()
    {
        HandleMovementWhenRecover?.Invoke();
        yield return new WaitForSeconds(comboResetTime);
        comboCounter = 0;
        _isAttackInputting = false;
        _holdTime = 0;
    }

    private void OnTryWeaponSwitch() // this function only get call when the OnWeaponSwitch() method get call
    {
        if (weaponList.Count <= 1) { return; }

        //Try get next weapon in list
        //Switch between 2 weapon in the list
        _weaponIndex = (_weaponIndex + 1) % weaponList.Count; // modulo to cycle through the weapon list
        bool activeState;
        for (int i = 0; i < weaponList.Count;)
        {
            activeState = (i == _weaponIndex) ? true : false;
            weaponList[i].weaponModel.SetActive(activeState);
            if (activeState)
            {
                _currentWeapon = weaponList[i];
                Debug.Log(_currentWeapon.name);
            }
            i++;
        }
    }

    private void OnTryPickUpWeapon() // this function only call when the OnPickUpWeapon() method get call
    {

        //Sphere cast To check for weapon
        //Compare distance when detect more than 1 weapon
        //Try pick up close weapon
        //If there is no space for more weapon throw the current equppied one and pick up the one on the ground else just pick up new weapon and add to list (maximum space: 2)
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, collectRange, layerToCheck);

        foreach (var hitCollider in hitColliders)
        {
            float distanceFromItemToPlayer = Vector3.Distance(hitCollider.transform.position, playerTransform.position);

            if (distanceFromItemToPlayer <= collectRange || distanceFromItemToPlayer <= collectRange && _currentWeapon == null)
            {
                WeaponBase weaponToAdd = hitCollider.GetComponentInChildren<WeaponBase>(); // will get the script of the weapon the sphere hit

                if (weaponList.Count >= _maxWeaponNum)
                {
                    //Replace weapon?   
                    weaponList.Remove(_currentWeapon);
                    _currentWeapon?.transform.SetParent(null, true);
                    _currentWeapon.GetComponent<BoxCollider>().enabled = true;
                    weaponList.Add(weaponToAdd);
                }
                else if (weaponList.Count < _maxWeaponNum || _currentWeapon == null)
                {
                    //weaponList.Add(weaponToAdd);
                    if (_currentWeapon != null)
                    {
                        _currentWeapon.weaponModel.SetActive(false);
                    }
                }
                // the whole section below is to set up the weapon we just pick up and set the boxCollider to false to immediately attack when pick up
                _currentWeapon = weaponToAdd;
                SetUpWeaponPosition(_currentWeapon, _weaponIndex);
            }
        }
    }

    private void SetUpWeaponPosition(WeaponBase currentWeapon, int weaponIndex)
    {
        currentWeapon?.transform.SetParent(this.transform);
        CurrentWeapon?.Invoke();
        weaponIndex = weaponList.IndexOf(currentWeapon);
        currentWeapon.transform.position = this.transform.position;
        currentWeapon.transform.rotation = this.transform.rotation;
        currentWeapon.GetComponentInChildren<BoxCollider>().enabled = false;
    }

    private void OnPickUpWeapon(InputAction.CallbackContext context) // this function will get call once when we pick up weapon (which is the "F" key)
    {
        if (context.performed)
        {
            OnTryPickUpWeapon();
        }
    }

    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnTryWeaponSwitch();
        }
    }
    //Handle Switching weapon
    //Handle picking up or removing weapon
    //Handle Weapon input

    void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, 5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, 4f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, 3f);
        }
    }

}