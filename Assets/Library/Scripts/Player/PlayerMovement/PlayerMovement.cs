using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public static event Action dashCancel;

    [Header("References")]
    private PlayerBase m_PlayerBase;
    protected Rigidbody _rb;
    private PlayerInput _playerInput;
    private Camera _camera;

    [Header("Movement")]
    //private float playerSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float standStillTimeAfterAttackRecover;
    private Vector3 moveDirection;
    private Vector2 _mousePosition;
    private Vector2 _movement;
    private bool _isOnSlope = false;

    [Header("Dash Manager")]
    [SerializeField] int maxCharge = 3;
    [SerializeField] int currentCharge;
    [SerializeField] private float dashRecoverTimePerCharge = 1f;
    [SerializeField] private float dashInputCooldown = 0.3f;
    [SerializeField] private float dashRegenMultiplier = 1.0f;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    public AnimationCurve animCurve;
    private bool canDash = true;
    private bool isDashing;
    private float dashRecoverTimePerChargeCount;

    [Header("Parry Manager")]
    [SerializeField] private Transform parryBoxRefPoint; // A transform point that will spawn the parry box
    [SerializeField] private Vector3 parryBoxSize = new Vector3(1, 1, 1);
    [SerializeField] private LayerMask bulletLayerMask; 
    [SerializeField] private float invulnerableTimerOrg = 0.6f;
    [SerializeField] private float standStillTimerOrg = 1f;
    public static event Action OnParryStart;
    public static event Action OnParryStop;
    private bool canParry = true;
    private bool isParrying = false;
    private Coroutine parryCoroutine;
    private Coroutine immobileCoroutine;

    [Header("Animation")]
    [SerializeField] private PlayerAnimation playerAnimation;
    
   
    private Coroutine dashCoroutine;
    private Coroutine stopMovementCoroutine;
    Quaternion _initialRotation;
    
    
    private bool isAttacking = false;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveDuration = 0.2f;
    public bool isRecovering = false;


    private void Awake()
    {
        _camera = Camera.main;
        _playerInput = new PlayerInput();
        currentCharge = maxCharge;
        m_PlayerBase = GetComponent<PlayerBase>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _playerInput.Player.Move.performed += Move;
        _playerInput.Player.Move.canceled += OnMoveCancel;
        _playerInput.Player.Dash.performed += Dash;
        _playerInput.Player.MousePos.performed += MousePos;
        _playerInput.Player.Parry.performed += Parry;
        _playerInput.Enable();
        WeaponManager.AttackHandle += OnMoveCharacterForward;
        WeaponManager.HandleMovementWhenRecover += StopMovementDuringRecoveryState;

        OnParryStart += () => isParrying = true;
        OnParryStop += () => isParrying = false;
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.performed -= Move;
        _playerInput.Player.Move.canceled -= OnMoveCancel;
        _playerInput.Player.Dash.performed -= Dash;
        _playerInput.Player.Parry.performed -= Parry;
        _playerInput.Disable();
        WeaponManager.AttackHandle -= OnMoveCharacterForward;
        WeaponManager.HandleMovementWhenRecover -= StopMovementDuringRecoveryState;

        OnParryStart -= () => isParrying = true;
        OnParryStop -= () => isParrying = false;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        RechargeDash();
    }

    private void FixedUpdate()
    {
        if (isParrying) return;
        if (isDashing) return;
        if (isAttacking) return;
        LookAtMousePosition();
        MoveCharacter();
    }

    #region INPUT_LISTENER

    private void MousePos(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();

    }

    public void Move(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    public void OnMoveCancel(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
        //_rb.velocity = new Vector3(0, _rb.velocity.y, 0); 
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(currentCharge <= 0 ) { return; }
        if (dashCoroutine == null && canDash)
        {
            dashCoroutine = StartCoroutine(Dash());
        }
    }

    public void Parry(InputAction.CallbackContext context)
    {
        if (parryCoroutine == null && canParry)
        {
            parryCoroutine = StartCoroutine(Parry());
        }
    }

    #endregion


    #region MOVEMENT
    private void MoveCharacter()
    {
        
        if (isRecovering || isAttacking)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            return;
        }

        moveDirection = new Vector3(_movement.x, 0, _movement.y).normalized;
        var playerMovement = moveDirection * PlayerDatas.Instance.GetStats.MoveSpeed;
        playerMovement.y = _rb.velocity.y;
        _rb.velocity = playerMovement;
        playerAnimation.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    }

    private void StopMovementDuringRecoveryState()
    {
        stopMovementCoroutine = StartCoroutine(WaitToReactivateMovementAfterRecovery());
    }

    private IEnumerator WaitToReactivateMovementAfterRecovery()
    {
        _rb.velocity = Vector3.zero;
        isRecovering = true;
        yield return new WaitForSeconds(standStillTimeAfterAttackRecover);
        isRecovering = false;
        stopMovementCoroutine = null;
    }

    private void OnMoveCharacterForward(int ComboCounter)
    {
        _rb.velocity = Vector3.zero;
        StartCoroutine(MoveCharacterForward(ComboCounter));
    }

    private IEnumerator MoveCharacterForward(int ComboCounter)
    {
        float timer = 0f;
        _rb.velocity = Vector3.zero;
        isAttacking = true;
        Vector3 forwardForce = transform.forward * (moveDistance / moveDuration);

        while (timer < moveDuration)
        {
            _rb.AddForce(forwardForce, ForceMode.Impulse);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _rb.velocity = Vector3.zero;
        isAttacking = false;
        stopMovementCoroutine = StartCoroutine(WaitToReactivateMovementAfterRecovery());
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
            else if (_rb.velocity.y > 0)
            {
                var velocity = _rb.velocity;
                velocity.y = 0;
                _rb.velocity = velocity;
            }
        }
    }

    #endregion

    #region DASH

    IEnumerator Dash()
    {
        CancelParry();
        _rb.velocity = Vector3.zero;
        canDash = false;
              
        Vector3 dashDirection = (_playerInput.Player.Move.IsPressed()) ? moveDirection : transform.forward;
        
        
        yield return new WaitForFixedUpdate();       
        float elapsedDashTime = 0f;
        isDashing = true;       
        while (elapsedDashTime < dashDuration)
        {
            _rb.AddForce(dashDirection * (dashForce / dashDuration) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            elapsedDashTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        dashCancel?.Invoke();
        dashCoroutine = null;
        isDashing = false;
        currentCharge--;

        yield return new WaitForSeconds(dashInputCooldown);
        canDash = true;      
    }

    private void RechargeDash()
    {
        if (currentCharge >= maxCharge) { return; }
        dashRecoverTimePerChargeCount += Time.deltaTime * dashRegenMultiplier;
        if (dashRecoverTimePerChargeCount > dashRecoverTimePerCharge)
        {
            currentCharge++;
            dashRecoverTimePerChargeCount = 0;
        }
    }

    #endregion

    #region PARRY

    private IEnumerator Parry()
    {
        canParry = false;

        OnParryStart?.Invoke();

        float elapsedInvulnerableTime = invulnerableTimerOrg;
        float elapsedStandStillTime = standStillTimerOrg;

        m_PlayerBase.StartImmunityCoroutine(invulnerableTimerOrg);

        Debug.Log("Parrying");
        playerAnimation.Parry();

        while (elapsedInvulnerableTime >= 0)
        {
            elapsedInvulnerableTime -= Time.deltaTime;

            Collider[] colliders = Physics.OverlapBox(parryBoxRefPoint.position, parryBoxSize / 2, transform.rotation, bulletLayerMask);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    GameObject actualBullet = colliders[i].transform.gameObject; 

                    EnemyProjectile bulletScript = actualBullet.GetComponent<EnemyProjectile>();

                    if (bulletScript != null)
                    {
                        bulletScript.ReflectBulletReverse();
                    }
                }
            }

            if (immobileCoroutine == null)
                immobileCoroutine = StartCoroutine(StandStill());

            yield return null;
        }

    }

    private void CancelParry()
    {
        StopCoroutine(StandStill());
        OnParryStop?.Invoke();
        canParry = true;       
        parryCoroutine = null;
        immobileCoroutine = null;
    }

    private IEnumerator StandStill()
    {
        _rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(standStillTimerOrg);

        OnParryStop?.Invoke();

        canParry = true;

        // Gotta make sure the coroutine is null to match the condition above! And also, cleans up each coroutine after each use
        parryCoroutine = null; 
        immobileCoroutine = null;
    }

    #endregion

    private void LookAtMousePosition() //Look at player mouse position
    {
        Vector3 _mousePos = Input.mousePosition;
        Vector3 CharacterPos = _camera.WorldToScreenPoint(transform.position);
        Vector3 dir = _mousePos - CharacterPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, -angle + 90, 0);
        //playerAnimation.RotateUpperBody(playerUpperSpine, dir);
    }


    void OnDrawGizmos()
    {
        if (parryBoxRefPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(parryBoxRefPoint.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, parryBoxSize);
        }
    }

}
