using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerActionState
{
    [Header("References")]
    private PlayerBase m_PlayerBase;
    protected Rigidbody _rb;
    private PlayerInput _playerInput;

    [Header("Movement")]
    //private float playerSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private float rotationSpeed;

    [Header("Dash Manager")]
    [SerializeField] int maxCharge = 3;
    [SerializeField] int currentCharge;
    [SerializeField] private float dashCooldown = 1.0f;
    [SerializeField] private float regenCooldown = 2.0f;
    [SerializeField] private float overheatCooldown = 5.0f;
    [SerializeField] private float dashDuration;


    //init
    private CapsuleCollider _capsuleCollider;
    private bool isOverheated = false;
    private Coroutine regenCoroutine;
    private Coroutine dashCoroutine;
    Quaternion _initialRotation;
    private Vector2 _movement;
    private bool _isOnSlope = false;
    private Camera _camera;
    private Vector2 _mousePosition;
    PlayerBase _playerStats; // Comment if don't use.
    

    private void Awake()
    {
        _camera = Camera.main;
        _playerInput = new PlayerInput();
        currentCharge = maxCharge;
        m_PlayerBase = GetComponent<PlayerBase>();
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        _playerInput.Player.Move.performed += Move;
        _playerInput.Player.Move.canceled += Move;
        _playerInput.Player.Dash.performed += Dash;
        _playerInput.Player.MousePos.performed += MousePos;
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.performed -= Move;
        _playerInput.Player.Move.canceled -= Move;
        _playerInput.Player.Dash.performed -= Dash;
        _playerInput.Disable();
    }

    void Update()
    {
        if (_playerInput.Player.Dash.WasPressedThisFrame())
        {
            if (dashCoroutine == null && currentCharge > 0)
            {
                dashCoroutine = StartCoroutine(Dash());
            }
        }

        if (regenCoroutine == null)  //Regen whenever not dashing
        {
            regenCoroutine = StartCoroutine(RegenCharge());
        }
    }

    private void FixedUpdate()
    {
        LookAtMousePosition();
        MoveCharacter();  
    }

    private void MoveCharacter()
    {
        var playerMovement = new Vector3(_movement.x, 0, _movement.y).normalized * PlayerDatas.Instance.GetStats.MoveSpeed;
        playerMovement.y = _rb.velocity.y;
        _rb.velocity = playerMovement;
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "GoingUp")
        {
            if (_playerInput.Player.Dash.IsPressed())
            {
                if (_rb.velocity.y > 0)
                {
                    var velocity = _rb.velocity;
                    velocity.y = 0;
                    _rb.velocity = velocity;
                }
            }
            else if(_rb.velocity.y > 0)
            {
                var velocity = _rb.velocity;
                velocity.y = 0;
                _rb.velocity = velocity;
            }
        }
    }
    IEnumerator Dash()
    {
        currentCharge--;
        //DASH GOES HERE
        Vector3 dashDirection = new Vector3(_movement.x, 0, _movement.y).normalized;
        float elapsedDashTime = 0f;

        while (elapsedDashTime < dashDuration)
        {
            _rb.AddForce(dashDirection * (dashForce / dashDuration) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            elapsedDashTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        dashCoroutine = null;

        //Overheat check
        if (currentCharge == 0)
        {
            StartCoroutine(HandleOverheat());
        }
    }

    private IEnumerator RegenCharge()
    {
        yield return new WaitForSeconds(regenCooldown);
        if (currentCharge < maxCharge && !isOverheated) //avoid overload
        {
            currentCharge++;
        }
        regenCoroutine = null;  // Reset coroutine reference when regen is done
    }

    private IEnumerator HandleOverheat()
    {
        isOverheated = true;
        yield return new WaitForSeconds(overheatCooldown);
        isOverheated = false;
    }


    private void LookAtMousePosition() //Look at player mouse position
    {
        //Vector3 directionFromCharacterToMouse = _mousePosition - transform.position;
        //Vector3 mousePosition = _camera.WorldToScreenPoint(directionFromCharacterToMouse);

        //directionFromCharacterToMouse.y = 0f;


        //if (mousePosition != Vector3.zero)
        //{
        //    Quaternion rotation = Quaternion.LookRotation(mousePosition);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        //}
        Vector3 _mousePos = Input.mousePosition;
        Vector3 CharacterPos = _camera.WorldToScreenPoint(transform.position);
        Vector3 dir = _mousePos - CharacterPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, -angle + 90, 0); 

    }
    private void MousePos(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();

    }

    public void Move(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    public void Dash(InputAction.CallbackContext context)
    {

    }
}
