using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private PlayerBase player;

    public delegate void CurrentWeaponHandler();
    public static event CurrentWeaponHandler CurrentWeapon;

    public delegate void OnHandlingAttack(int ComboCounter);
    public static event OnHandlingAttack AttackHandle;
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
    private bool _isNormalAttack = true;
    public bool isAttack = false;
    private float cooldownTimer = 0f;
    public bool isDashingToCancelAction = false;
    public bool isAllowToCancelAction = false;

    //charge attack timer
    [SerializeField] private float _holdTime = 0f;
    private bool _isHoldAttack = false;
    private bool _startHold = false;
    private float comboAttackSpeed = 0.3f;
    
    /*IF you want to make the hold attacks. Try make a delay BEFORE starting the hold check
     This prevent player from registering normal attack as charge attack
     */
    [Header("ComboSystemAndAttack")]
    [SerializeField] int comboCounter = 0;
    int maxComboCount = 3;
    float comboResetTime = 0.6f;
    float recoverDuration = 2f;
    float recoverTimer = 0f;
    public bool isRecovering = false;

    [Header("WeaponCollectRange")]
    [SerializeField] private float collectRange;

    private Transform playerTransform;
    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float _innitNormalToChargeAttackDelay; // This one
    //[SerializeField] private WeaponData swordData;
    //[SerializeField] private GameObject templateWeaponModel;
    //------------------------

    //Since each weapon have different delay time, need to overwrite the SO_WeaponData each time _currentWeapon value changes

    private void Awake()
    {
        _playerInput = new PlayerInput();
        playerTransform = transform.parent ?? transform;
        _currentWeapon = GetComponentInChildren<WeaponBase>();
        PlayerMovement.dashCancel += DashingToCancelAction;
        if(_currentWeapon != null)
        {
            weaponList.Add(_currentWeapon);
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
        if (_startHold && !isRecovering) // hold timer for charge attack
        {
            _holdTime += Time.deltaTime;
            if(_holdTime >= _currentWeapon._weaponData.holdThreshold) // if the player hold the attack button long enough or longer than the threshold give by the current weapon
            {                                                           //then it will notice the system to know that it is a hold attack which will notice the OnAttackInputEnd()
                _isHoldAttack = true;                                   // method that it is a hold attack (_isHoldAttack) 
                _isNormalAttack = false;
            }
            if (isDashingToCancelAction)
            {
                _isHoldAttack = false;
                _startHold = false;
                _holdTime = 0;
                isAllowToCancelAction = false;
            }
        }
        if (isAttack) // cooldown timer betweeen attack and between normal attack and charge attack
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                _isNormalAttack = true;
                isAttack = false;
            }
        }
        if(isRecovering)
        {
            recoverTimer -= Time.deltaTime;
            if (recoverTimer <= 0f)
            {
                isRecovering = false;
            }
        }
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
        if(_currentWeapon != null)
        {
            _currentWeapon = StartingWeapon;
            _currentWeapon.GetComponent<BoxCollider>().enabled = false;
        } 
    }

    private void DashingToCancelAction()
    {
        if(isAllowToCancelAction)
        {
            isDashingToCancelAction = true;
            comboCounter = 0;
        }
    }

    private void OnAttackInputPerform(InputAction.CallbackContext context)
    {
        if (_currentWeapon == null || isRecovering)
        {
            Debug.Log("there is currently no weapon or player is recovering");
            return;
        }
        _startHold = true;
        isAllowToCancelAction = true;
        _holdTime = 0f; // always set the _holdTimer back to 0 when start click to avoid accumulate holdTime if player just do normal attack
    }

    private void OnAttackInputEnd(InputAction.CallbackContext context) // this is Unity new input event for when the player release the mouse button
    {
        if( _currentWeapon != null)
        {
            if(isDashingToCancelAction)
            {
                isDashingToCancelAction = false;
                return;
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
                _startHold = false;
                comboCounter = 0;
            }
            else if(!isAttack && !isRecovering) //if going for combo just focus on this statement
            {
                if (isRecovering) { return; }

                _currentWeapon.OnInnitNormalAttack();
                
                
                if(comboCounter > maxComboCount)
                {
                    recoverTimer = recoverDuration;
                    isRecovering = true;
                    StartCoroutine(ResetFullCombo());
                    return;
                }
                
                cooldownTimer = _currentWeapon._weaponData.attackSpeed;

                if(comboCoroutine != null)
                {
                    StopCoroutine(comboCoroutine);
                }

                cooldownTimer = comboAttackSpeed;
                comboCounter++;
                AttackHandle?.Invoke(comboCounter);
                isAttack = true;
                comboCoroutine = StartCoroutine(ResetCombo());
            }
            
            //isAttack = true; // set the isAttack = true again so that it will start cooldown, avoid attack with no cooldown
            ResetAttackState();
        }
        //_isHoldAttack = true then charge attack and set the cooldown to weapon normal/charge attack speed and then start cooldown   
    }

    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(comboResetTime);

        isDashingToCancelAction = false;
        isAllowToCancelAction = false;

        recoverTimer = recoverDuration;
        isRecovering = true;
        isAttack = true;
        cooldownTimer = _currentWeapon._weaponData.attackSpeed;
        comboCounter = 0;
        _startHold = false;
        _holdTime = 0f;
    }

    private IEnumerator ResetFullCombo()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboCounter = 0;
        _startHold = false;
        _holdTime = 0;
    }

    private void ResetAttackState()
    {
        _holdTime = 0f;
        _startHold = false;
        _isHoldAttack = false;
        _isNormalAttack = false;
    }

    private void OnTryWeaponSwitch() // this function only get call when the OnWeaponSwitch() method get call
    {
        if(weaponList.Count <= 1) { return; }

        //Try get next weapon in list
        //Switch between 2 weapon in the list
        _weaponIndex = (_weaponIndex + 1) % weaponList.Count; // modulo to cycle through the weapon list
        bool activeState;
        for (int i = 0; i < weaponList.Count;)
        {
            activeState = (i == _weaponIndex) ? true : false; 
            weaponList[i].weaponModel.SetActive(activeState);
            if(activeState)
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
                else if(weaponList.Count < _maxWeaponNum || _currentWeapon == null) 
                {
                    weaponList.Add(weaponToAdd);
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
        if(playerTransform != null)
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
