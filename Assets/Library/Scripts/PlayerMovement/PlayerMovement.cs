using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Moving,
    Dashing,
    StandDashing
}

public class PlayerMovement : MonoBehaviour
{
      private PlayerInput _playerInput;
      private float _dashDuration;
      [SerializeField] private float dashMaxDuration;
      [SerializeField] private float dashForce;
      [SerializeField] private float playerSpeed;
      [SerializeField] private float timeBetweenDashes;
      [SerializeField] private bool allowToDashWhenStandStill;
      private bool _isPressDash;
      private bool _canDash;
      private Vector2 _movement;
      private Rigidbody _rb;
      private PlayerState _state;
      private Camera _camera;
      private Ray _ray;
      private Vector2 _mousePosition;
  
      private void Awake()
      {
          _camera = Camera.main;
          _canDash = true;
          _rb = GetComponent<Rigidbody>();
          MoveToState(PlayerState.Idle);
          _playerInput = new PlayerInput();
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
          LookAtMousePosition();
          switch (_state)
          {
              case PlayerState.Idle:
                  if (_movement != Vector2.zero)
                  {
                      MoveToState(PlayerState.Moving);
                  }
                  else if (_playerInput.Player.Dash.inProgress && _canDash)
                  {
                      MoveToState(PlayerState.StandDashing);
                  }
                  break;
              case PlayerState.Moving:
                  if (_movement == Vector2.zero)
                  {
                      MoveToState(PlayerState.Idle);
                  }
                  else if (_playerInput.Player.Dash.inProgress && _canDash)
                  {
                      Debug.Log("Dashing");
                      MoveToState(PlayerState.Dashing);
                  }
                  break;
              case PlayerState.Dashing:
                  break;
              case PlayerState.StandDashing:
                  break;
          }
      }
  
      private void FixedUpdate()
      {
          switch (_state)
          {
              case PlayerState.Moving:
                  Vector3 playerMovement = new Vector3(_movement.x, 0, _movement.y) * playerSpeed;
                  _rb.velocity = playerMovement;
                  break;
              case PlayerState.Dashing:
                  DashAction();
                  break;
              case PlayerState.StandDashing:
                  DashAction();
                  break;
          }
      }
  
      public void Move(InputAction.CallbackContext context)
      {
          _movement = context.ReadValue<Vector2>();
      }
  
      public void Dash(InputAction.CallbackContext context)
      {
          if (context.performed)
          {
              Debug.Log("pressed dash");
              _isPressDash = true;
          }
          else if (context.canceled)
          {
              _isPressDash = false;
          }
      }
      
      private void MousePos(InputAction.CallbackContext context)
      {
          _mousePosition = context.ReadValue<Vector2>();
      }
  
      private void MoveToState(PlayerState newState)
      {
          _state = newState;
          switch (newState)
          {
              case PlayerState.Idle:
                  _rb.velocity = Vector3.zero;
                  break;
              case PlayerState.Moving:
                  break;
              case PlayerState.Dashing:
                  StartDash();
                  break;
              case PlayerState.StandDashing:
                  StartStandDash();
                  break;
          }
      }
  
      private void StartDash()
      {
          Debug.Log("Hey");
          _canDash = false;
          _dashDuration = dashMaxDuration;
          _rb.velocity = new Vector3(_movement.x, 0, _movement.y).normalized * dashForce;
      }
  
      private void StartStandDash()
      {
          _canDash = false;
          _dashDuration = dashMaxDuration;
          Ray ray = _camera.ScreenPointToRay(_mousePosition);
          if (allowToDashWhenStandStill)
          {
              if (Physics.Raycast(ray, out RaycastHit hit))
              {
                  _rb.velocity = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z).normalized * dashForce;
              } 
          }
          
      }
  
      private void DashAction()
      {
          _dashDuration -= Time.deltaTime;
          if (_dashDuration <= 0)
          {
              Debug.Log(_dashDuration);
              _rb.velocity = Vector3.zero;
              MoveToState(PlayerState.Idle);
              StartCoroutine(DashCooldown());
          }
  
      }
  
      IEnumerator DashCooldown()
      {
          yield return new WaitForSeconds(timeBetweenDashes);
          _canDash = true;
      }
      
      private void LookAtMousePosition()
      {
          _ray = _camera.ScreenPointToRay(_mousePosition);
          RaycastHit hit;
          if (Physics.Raycast(_ray, out hit))
          {
              Vector3 playerToMouse = hit.point - transform.position;
              playerToMouse.y = 0;
              Quaternion rotation = Quaternion.LookRotation(playerToMouse);
              _rb.MoveRotation(rotation);
          }
      }
}
