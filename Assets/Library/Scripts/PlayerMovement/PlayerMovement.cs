using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Moving,
    Dashing,
}

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject _playerBody;
    private PlayerInput _playerInput;

    [Header("Movement")]
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private float rotationSpeed;

    [Header("Dash Manager")]
    [SerializeField] int maxCharge = 3;
    [SerializeField] int currentCharge;
    [SerializeField] private float dashCooldown = 1.0f;
    [SerializeField] private float regenCooldown = 2.0f;
    [SerializeField] private float overheatCooldown = 5.0f;


    //init
    private bool isOverheated = false;
    private Coroutine regenCoroutine;
    private Coroutine dashCoroutine;

    private Vector2 _movement;
    private PlayerState _state;
    private Camera _camera;
    private Vector2 _mousePosition;


    private void Awake()
    {
        _camera = Camera.main;
        MoveToState(PlayerState.Idle);
        _playerInput = new PlayerInput();
        currentCharge = maxCharge;
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
        // If the player in the current state and meet the condition then move to the next state using MoveToState() method
        switch (_state)
        {
            case PlayerState.Idle:
                if (_movement != Vector2.zero)
                {
                    MoveToState(PlayerState.Moving);
                }
                break;
            case PlayerState.Moving:
                if (_movement == Vector2.zero)
                {
                    MoveToState(PlayerState.Idle);
                }
                else if (_playerInput.Player.Dash.WasPressedThisFrame())
                {
                    MoveToState(PlayerState.Dashing);
                }
                break;
            case PlayerState.Dashing:
                break;
        }

        if (regenCoroutine == null)  //Regen whenever not dashing
        {
            regenCoroutine = StartCoroutine(RegenCharge());
        }
    }

    private void FixedUpdate()
    {
        LookAtMousePosition();
        switch (_state) // this is action if they are in the state then what will happen to the character and in a FixedUpdate to handle physics
        {
            case PlayerState.Idle:
                _rb.velocity = Vector3.zero;
                break;
            case PlayerState.Moving:
                Vector3 playerMovement = new Vector3(_movement.x, 0, _movement.y) * playerSpeed;
                _rb.velocity = playerMovement;
                break;
            case PlayerState.Dashing:
                if (dashCoroutine == null && currentCharge > 0)
                {
                    dashCoroutine = StartCoroutine(Dash());
                }
                else
                {
                    MoveToState(PlayerState.Idle);
                }
                break;
        }
    }

    IEnumerator Dash()
    {

        currentCharge--;
        //DASH GOES HERE
        _rb.AddForce(new Vector3(_movement.x, 0, _movement.y).normalized * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dashCooldown);
        MoveToState(PlayerState.Idle);
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

    private void MoveToState(PlayerState newState) 
    {
        _state = newState;
        switch (newState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Moving:
                break;
            case PlayerState.Dashing:
                break;
        }
    }


    private void LookAtMousePosition() //Look at player mouse position
    {
        Vector3 mousePosition = _camera.ScreenToWorldPoint(new Vector3(_mousePosition.x, _mousePosition.y, _camera.transform.position.y));
        Vector3 directionFromCharacterToMouse = mousePosition - _playerBody.transform.position;
        directionFromCharacterToMouse.y = 0f;

        if (directionFromCharacterToMouse != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(directionFromCharacterToMouse);
            _playerBody.transform.rotation = Quaternion.Slerp(_playerBody.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
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
        if (context.performed)
        {
        }
        else if (context.canceled)
        {
        }
    }
}
