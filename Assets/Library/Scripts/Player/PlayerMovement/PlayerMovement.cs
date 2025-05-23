using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static event Action dashCancel;
    public static event Action dashIndicate;
    public static event Action OnDashUsed;
    public static event Action OnMaxChargeChanged;
    public static event Action OnDashRecoverTimeChanged;

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


    [Header("Coroutines")]
    private Coroutine dashCoroutine;
    private Coroutine stopMovementCoroutine;
    private Coroutine dashChargeUpdateCoroutine = null;


    [Header("Values")]
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveDuration = 0.2f;
    private bool isAttacking = false;
    public bool isRecovering = false;
    private float tempCharge = 0f;
    Quaternion _initialRotation;

    [Header("CharacterRotation")]
    [SerializeField] private bool allow8DirectionWalk;
    [SerializeField] private float rotateSpeed;
    
    // for UI 
    public float totalDashTime { get; private set; }
    public float CurrentCharge => currentCharge;
    public float DashRecoverTimePerChargeCount => dashRecoverTimePerChargeCount;
    public float DashRecoverTimePerCharge => dashRecoverTimePerCharge;

    private void Awake()
    {
        _camera = Camera.main;
        _playerInput = new PlayerInput();
        currentCharge = maxCharge;
        m_PlayerBase = GetComponent<PlayerBase>();
        _rb = GetComponent<Rigidbody>();
        totalDashTime = dashRecoverTimePerCharge * maxCharge;
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
        //if (dashChargeUpdateCoroutine != null)
        //{
        //    StopCoroutine(dashChargeUpdateCoroutine);
        //    dashChargeUpdateCoroutine = null;
        //}

        //if (dashChargeUpdateCoroutine == null)
        //dashChargeUpdateCoroutine = StartCoroutine(DashChargeModify(true, maxCharge));
    }

    private void Update()
    {
        RechargeDash();


        if (Input.GetKeyDown(KeyCode.Y))
        {
            Test();
        }
    }

    private void FixedUpdate()
    {
        if (isParrying) return;
        if (isDashing) return;
        if (isAttacking) return;
        if (!allow8DirectionWalk)
        {
            LookAtMousePosition();
        }
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
        if (currentCharge <= 0) { return; }
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
        if (GameManager.Instance.isPlayerDead || GameManager.Instance.inOverviewMode)
        {
            return;
        }
        if (isRecovering || isAttacking)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            return;
        }

        moveDirection = new Vector3(_movement.x, 0, _movement.y).normalized;
        var playerMovement = moveDirection * PlayerDatas.Instance.GetStats.MoveSpeed;
        playerMovement.y = _rb.velocity.y;
        _rb.velocity = playerMovement;
        if (allow8DirectionWalk && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }
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
        OnDashUsed?.Invoke();

        //if (dashChargeUpdateCoroutine != null)
        //{
        //    StopCoroutine(dashChargeUpdateCoroutine);
        //    dashChargeUpdateCoroutine = null;
        //}

        //if (dashChargeUpdateCoroutine == null)
        //dashChargeUpdateCoroutine = StartCoroutine(DashChargeModify(false, 1));

        yield return new WaitForSeconds(dashInputCooldown);
        canDash = true;
    }

    private void RechargeDash()
    {
        if (currentCharge >= maxCharge) { return; }

        dashRecoverTimePerChargeCount += Time.deltaTime * dashRegenMultiplier;
        OnDashRecoverTimeChanged?.Invoke();

        if (dashRecoverTimePerChargeCount > dashRecoverTimePerCharge)
        {
            currentCharge++;
            dashIndicate?.Invoke();
            dashRecoverTimePerChargeCount = 0;

            //if (dashChargeUpdateCoroutine != null)
            //{
            //    StopCoroutine(dashChargeUpdateCoroutine);
            //    dashChargeUpdateCoroutine = null;
            //}

            //if (dashChargeUpdateCoroutine == null)
            //    dashChargeUpdateCoroutine = StartCoroutine(DashChargeModify(true, 1));
        }
    }

    private IEnumerator DashChargeModify(bool increase, int amount)
    {
        float tempTarget = increase ? currentCharge + amount : currentCharge - amount; // Correctly assigns target without modifying currentCharge prematurely

        float smoothTime = increase ? dashRecoverTimePerCharge / 10f : 0.0075f; // Correctly assigns target without modifying currentCharge prematurely

        tempCharge = currentCharge;

        float placeHolderVal = 0f;

        float currentVel = 0f;

        while (true)
        {
            // Smoothly transition tempCharge towards tempTarget
            placeHolderVal = Mathf.SmoothDamp(tempCharge, tempTarget, ref currentVel, smoothTime);

            tempCharge = placeHolderVal;

            Debug.Log(tempCharge);

            // Only update currentCharge when it's close enough to the target
            if (Mathf.Abs(tempCharge - tempTarget) < 0.0025f)
            {
                tempCharge = tempTarget; // Snap to target

                currentCharge = increase ? (int)tempCharge : (int)Mathf.Floor(tempCharge); // Update integer charge level once transition completes

                break;
            }

            //Debug.Log("Temp Charge: " + tempCharge);
            //Debug.Log("Current Charge: " + currentCharge);

            yield return null;
        }

        if (increase) dashIndicate?.Invoke();

        dashChargeUpdateCoroutine = null;
    }


    private void Test()
    {
        maxCharge++;
        OnMaxChargeChanged?.Invoke();
    }


    public int GetMaxCharge()
    {
        return maxCharge;
    }


    public float GetDashChargeProgress() // For displaying dash charge UI
    {
        return Mathf.InverseLerp(0f, maxCharge, tempCharge);
    }   

 

    public float GetDashRecoverTimePerCharge()
    {
        return dashRecoverTimePerCharge;
    }

    public float GetDashRecoverTimePerChargeCount()
    {
        return dashRecoverTimePerChargeCount;
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
        if (GameManager.Instance.isPlayerDead || GameManager.Instance.inOverviewMode)
        {
            return; // Stop rotation while teleport menu or death screen is open
        }
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
