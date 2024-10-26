using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public List<WeaponBase> weaponList = new List<WeaponBase>();

    public PlayerInput _playerInput;

    private WeaponBase _currentWeapon;
    private int _maxWeaponNum = 2;
    private int _weaponIndex = 0;
    private bool _isNormalAttack = true;
    private bool isAttack = false;
    private float cooldownTimer = 0f;
    private float _holdTime = 0f;
    private bool _isHoldAttack = false;
    private bool _startHold = false;
    /*IF you want to make the hold attacks. Try make a delay BEFORE starting the hold check
     This prevent player from registering normal attack as charge attack
     */
    [SerializeField] private LayerMask layerToCheck;
    private Transform playerTransform;
    [SerializeField] private float _innitNormalToChargeAttackDelay; // This one
    [SerializeField] private float collectRange;
    //[SerializeField] private WeaponData swordData;
    //[SerializeField] private GameObject templateWeaponModel;
    //------------------------

    //Since each weapon have different delay time, need to overwrite the SO_WeaponData each time _currentWeapon value changes

    private void Awake()
    {
        _playerInput = new PlayerInput();
        playerTransform = transform.parent ?? transform;
        _currentWeapon = GetComponentInChildren<WeaponBase>();
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

    private void Update()
    {
        if (_startHold)
        {
            _holdTime += Time.deltaTime;
            //Debug.Log(_holdTime);
            if(_holdTime >= _currentWeapon._weaponData.holdThreshold)
            {
                _isHoldAttack = true;
                _isNormalAttack = false;
            }  
        }
        if (isAttack)
        {
            cooldownTimer -= Time.deltaTime;
            Debug.Log("cooldown: " + cooldownTimer);
            if (cooldownTimer <= 0f)
            {
                _isNormalAttack = true;
                isAttack = false;
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
        
        _currentWeapon = StartingWeapon;
        _currentWeapon.GetComponent<BoxCollider>().enabled = false;
    }

    //Just add more stuff in here so it keep track of the time for charge attack and cooldown
    private void OnAttackInputPerform(InputAction.CallbackContext context)
    {
        if (_currentWeapon == null)
        {
            Debug.Log("there is currently no weapon");
            return;
        }
        _startHold = true;
        _holdTime = 0f;
    }

    private void OnAttackInputEnd(InputAction.CallbackContext context)
    {
        Debug.Log("release");
        if (_isHoldAttack)
        {
            Debug.Log("ChargeAttack");
            _currentWeapon.OnInnitSecondaryAttack();
            _startHold = false;
            cooldownTimer = _currentWeapon._weaponData.chargeAttackSpeed;
        }
        else if (_isNormalAttack)
        {
            Debug.Log("Attack");
            _currentWeapon.OnInnitNormalAttack(); //Like this
            cooldownTimer = _currentWeapon._weaponData.attackSpeed;
        }
        isAttack = true;
        ResetAttackState();
    }

    private void ResetAttackState()
    {
        _holdTime = 0f;
        _startHold = false;
        _isHoldAttack = false;
        _isNormalAttack = false;
    }

    private void OnTryWeaponSwitch()
    {
        if(weaponList.Count <= 1) { return; }

        //Try get next weapon in list

        //_weaponIndex++;
        //_weaponIndex = Mathf.Clamp(_weaponIndex, 0, weaponList.Count - 1);
        _weaponIndex = (_weaponIndex + 1) % weaponList.Count;
        bool activeState;
        for (int i = 0; i < weaponList.Count;)
        {
            activeState = (i == _weaponIndex) ? true : false;
            weaponList[i].weaponModel.SetActive(activeState);
            if(activeState)
            {
                _currentWeapon = weaponList[i];
            }
            i++;
        }

    }

    private void OnTryPickUpWeapon()
    {
        //if(weaponList.Count == _maxWeaponNum) { return; }
        //Sphere cast To check for weapon
        //Compare distance when detect more than 1 weapon
        //Try pick up close weapon
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, collectRange, layerToCheck);
        foreach (var hitCollider in hitColliders)
        {
            float distanceFromItemToPlayer = Vector3.Distance(hitCollider.transform.position, playerTransform.position);
            if (distanceFromItemToPlayer <= collectRange)
            {
                Debug.Log(hitCollider.gameObject.name);
                WeaponBase weaponToAdd = hitCollider.GetComponentInChildren<WeaponBase>();
                if (weaponList.Count >= _maxWeaponNum)
                {
                    //Replace weapon? 
                    weaponList.Remove(_currentWeapon);
                    _currentWeapon.transform.SetParent(null, true);
                    _currentWeapon.GetComponent<BoxCollider>().enabled = true;
                    weaponList.Add(weaponToAdd);
                }
                else
                {
                    weaponList.Add(weaponToAdd);
                    _currentWeapon.weaponModel.SetActive(false);
                }
                _currentWeapon = weaponToAdd;
                _weaponIndex = weaponList.IndexOf(_currentWeapon);
                _currentWeapon.transform.SetParent(transform);
                _currentWeapon.transform.position = this.transform.position;
                _currentWeapon.transform.rotation = this.transform.rotation;
                _currentWeapon.GetComponentInChildren<BoxCollider>().enabled = false;
                Debug.Log("current weapon: " + _currentWeapon);
            }
        }
    }

    private void OnPickUpWeapon(InputAction.CallbackContext context)
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
            //Debug.Log("switch");
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
