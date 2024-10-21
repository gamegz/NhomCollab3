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

    /*IF you want to make the hold attacks. Try make a delay BEFORE starting the hold check
     This prevent player from registering normal attack as charge attack
     */
    [SerializeField] private float _innitNormalToChargeAttackDelay; // This one
    [SerializeField] private WeaponData swordData;
    [SerializeField] private GameObject templateWeaponModel;
    //------------------------

    //Since each weapon have different delay time, need to overwrite the SO_WeaponData each time _currentWeapon value changes

    private void Awake()
    {
        _playerInput = new PlayerInput();
        weaponList.Add(_currentWeapon);
        if (weaponList.Count > 0)
        {
            //Innit(weaponList[0]);
            _currentWeapon = weaponList[0];
            //Debug.Log(_currentWeapon.weaponModel);
            Debug.Log(weaponList.Count);
        }
        else
        {
            //Debug.Log("weaponList is empty");
        }
    }

    void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.Player.Attack.performed += OnAttackInputPerform;
        _playerInput.Player.Attack.canceled += OnAttackInputEnd;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.Player.Attack.performed -= OnAttackInputPerform;
        _playerInput.Player.Attack.canceled -= OnAttackInputEnd;

    }

    private void Innit(WeaponBase StartingWeapon)
    {
        //Set current weapon
        //Enable Current weapon
        
        _currentWeapon = StartingWeapon;
        //_currentWeapon.weaponModel.SetActive(true);
    }

    //Just add more stuff in here so it keep track of the time for charge attack and cooldown
    private void OnAttackInputPerform(InputAction.CallbackContext context)
    {
        if (_currentWeapon == null)
        {
            //Debug.Log("there is currently no weapon");
            return;
        }
        _currentWeapon.OnInnitNormalAttack(); //Like this
    }



    private void OnAttackInputEnd(InputAction.CallbackContext context)
    {
        
    }

    private void OnTryWeaponSwitch()
    {
        if(weaponList.Count <= 1) { return; }

        //Try get next weapon in list
        _weaponIndex++;
        _weaponIndex = Mathf.Clamp(_weaponIndex, 0, weaponList.Count - 1);

        bool activeState;
        for (int i = 0; i < weaponList.Count;)
        {
            activeState = (i == _weaponIndex) ? true : false;
            weaponList[i].weaponModel.SetActive(activeState);
            i++;
        }

        if (_weaponIndex == weaponList.Count - 1)
        {
            _weaponIndex = 0;
        }
    }

    private void OnTryPickUpWeapon()
    {
        if(weaponList.Count == _maxWeaponNum) { return; }
        //Sphere cast To check for weapon
        //Compare distance when detect more than 1 weapon
        //Try pick up close weapon
        WeaponBase weaponToAdd = null;
        if (weaponList.Contains(weaponToAdd))
        {
            //Replace weapon? 
            int existingWeaponindex = weaponList.FindIndex(w => w == weaponToAdd);
            weaponList.RemoveAt(existingWeaponindex);
            weaponList.Insert(existingWeaponindex, weaponToAdd);
        }
        else
        {
            weaponList.Add(weaponToAdd);
        }
        _currentWeapon = weaponToAdd;
    }


    //Handle Switching weapon
    //Handle picking up or removing weapon
    //Handle Weapon input
}
