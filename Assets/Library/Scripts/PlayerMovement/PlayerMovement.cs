using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Player Movement Attribute")]
    [SerializeField] float speed;
    [Header("Player Dash Attribute")]
    [SerializeField] float dashForce;
    [SerializeField] float dashForceWhenNotMove;
    [SerializeField] private float dashDelay;
    [SerializeField] private bool allowDashWhenStandStill;
    
    private Camera _cameraMain;
    private bool _isDashing;
    private Ray _ray;
    private bool _canDash = true;
    private PlayerInput _input;
    Rigidbody _rigidbody;
    Vector2 _movement;
    Vector2 _mousePosition;
    private float _initDrag;
    private float _modifiedDrag = 4f;
    
    private void Awake()
    {
        _input = new PlayerInput();
        _rigidbody = GetComponent<Rigidbody>();
        _cameraMain = Camera.main;
        transform.hasChanged = false;
        _initDrag = _rigidbody.drag;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        
        if (!_isDashing)
        {
            Move(); // can only move when the player is not dashing
        }
        LookAtMousePosition();
    }

    #region Move

    private void Move() // move the player around
    {
        transform.position += new Vector3(_movement.x, 0, _movement.y) * (speed * Time.deltaTime);
    }
    #endregion

    #region DashInput Handling
    
    public void Dash(InputAction.CallbackContext context) // this function take the dash input, use it like assign button, when button press function run 
    {
        if (context.performed && !_isDashing && _canDash) // check if player press the button and if they can dash or not
        {
            if ((_input.Player.Move.ReadValue<Vector2>().sqrMagnitude > 0f)) // if the player is moving or not to dash
            {
                _isDashing = true;
                _canDash = false;
                Debug.Log("Dashing");
                DashAction(new Vector3(_movement.x, 0, _movement.y).normalized, dashForce);
                
            }
            else if ((_input.Player.Move.ReadValue<Vector2>().sqrMagnitude <= 0f && allowDashWhenStandStill)) // if the player is not moving and the allow Dash When Stand Still tick box get ticked then allow to dash when stand still
            {
                Ray ray = _cameraMain.ScreenPointToRay(_mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _isDashing = true;
                    _canDash = true;
                    Vector3 dashWhenNotMove = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z).normalized;
                    DashAction(dashWhenNotMove, dashForceWhenNotMove);
                    
                }
                
            }
        }
    }
    
    #endregion
    
    #region Dash Action
    private void DashAction(Vector3 dashDirection, float force) //Start Dashing
    {
        _rigidbody.velocity = dashDirection * force;
        _rigidbody.drag = _modifiedDrag;
        _isDashing = false;
        Invoke(nameof(DashReset), dashDelay); //Delay an amount of time then call Dash Reset function
    }
    

    private void DashReset() // reset the Dash
    {
        _rigidbody.drag = _initDrag;
        _canDash = true;
        _rigidbody.velocity = Vector3.zero;
    }
    #endregion

    #region Rotate Toward Mouse Position

    private void LookAtMousePosition() // look toward mouse position like Ruiner
    {
        _ray = _cameraMain.ScreenPointToRay(_mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit))
        {
            Vector3 playerToMouse = hit.point - transform.position;
            playerToMouse.y = 0;
            Quaternion rotation = Quaternion.LookRotation(playerToMouse);
            _rigidbody.MoveRotation(rotation);
        }
    }
    #endregion
    
    #region Movement Handling
    private void OnMovement(InputAction.CallbackContext context) // this function record the value of player keyboard then use it for moving
    {
        if (!_isDashing)
        {
            _movement = context.ReadValue<Vector2>();
        }
        
    }
    # endregion
    
    #region record the mouse position
    private void OnMousePos(InputAction.CallbackContext context) // record the mouse position
    {
        _mousePosition = context.ReadValue<Vector2>();
    }
    #endregion
    
    #region enable input and disable input
    private void OnEnable() // enable input
    {
        _input.Enable();
        _input.Player.Move.performed += OnMovement;
        _input.Player.Move.canceled += OnMovement;
        _input.Player.MousePos.performed += OnMousePos;
    }

    private void OnDisable() // disable input
    {
        _input.Disable();
    }
    #endregion
}
