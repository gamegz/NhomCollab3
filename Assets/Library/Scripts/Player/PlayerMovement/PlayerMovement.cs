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
    public AnimationCurve animCurve;

    [Header("Parry Manager")]
    [SerializeField] private Transform parryBoxRefPoint; // A transform point that will spawn the parry box
    [SerializeField] private Vector3 parryBoxSize = new Vector3(1, 1, 1); 
    [SerializeField] private LayerMask bulletLayerMask; // Detects bullet via layermask
    [SerializeField] private float invulnerableTimerOrg = 0.6f;
    [SerializeField] private float standStillTimerOrg = 1f;


    // [ INITIALIZATION ]


    // [ PARRY RELATED ]
    public static event Action OnParryStart;
    public static event Action OnParryStop;
    private bool canParry = true;
    private bool isParrying = false;
    private Coroutine parryCoroutine;
    private Coroutine immobileCoroutine;

    // [ ALL THE OTHER STUFF {Please Organize This Script For Me OMGGGGG}]
    private CapsuleCollider _capsuleCollider;
    private bool isOverheated = false;
    private bool _isOnSlope = false;
    private Coroutine regenCoroutine;
    private Coroutine dashCoroutine;
    private Coroutine stopMovementCoroutine;
    Quaternion _initialRotation;
    private Vector2 _mousePosition;
    private Vector2 _movement;
    private Camera _camera;
    private bool isAttacking = false;
    private float moveDistance = 2f;
    private float moveDuration = 0.2f;
    private float delayBeforeBackToWalking = 1f;
    public bool isRecovering = false;


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
        _playerInput.Player.Move.canceled -= Move;
        _playerInput.Player.Dash.performed -= Dash;
        _playerInput.Player.Parry.performed -= Parry;
        _playerInput.Disable();
        WeaponManager.AttackHandle -= OnMoveCharacterForward;
        WeaponManager.HandleMovementWhenRecover -= StopMovementDuringRecoveryState;

        OnParryStart -= () => isParrying = true;
        OnParryStop -= () => isParrying = false;
    }

    private void OnDestroy()
    {
        WeaponManager.AttackHandle -= OnMoveCharacterForward;
        WeaponManager.HandleMovementWhenRecover -= StopMovementDuringRecoveryState;
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

        if (_playerInput.Player.Parry.WasPressedThisFrame()) // Was pressed this frame is cool
        {
            if (parryCoroutine == null && canParry)
            {
                parryCoroutine = StartCoroutine(Parry());
            }
            else { Debug.Log("Apparently...");  }
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
        if (isParrying) return;;
        if (isRecovering || isAttacking)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            return;
        }

        var playerMovement = new Vector3(_movement.x, 0, _movement.y).normalized * PlayerDatas.Instance.GetStats.MoveSpeed;
        playerMovement.y = _rb.velocity.y;
        _rb.velocity = playerMovement;
    }

    private void StopMovementDuringRecoveryState()
    {
        stopMovementCoroutine = StartCoroutine(WaitToReactivateMovementAfterRecovery());
    }

    private IEnumerator WaitToReactivateMovementAfterRecovery()
    {
        _rb.velocity = Vector3.zero;
        isRecovering = true;
        yield return new WaitForSeconds(1.2f);
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
        _rb.velocity = Vector3.zero;
        yield return new WaitForFixedUpdate();
        Vector3 dashDirection = new Vector3(_movement.x, 0, _movement.y).normalized;
        float elapsedDashTime = 0f;

        float dashSpeedWhenStandStill = dashForce / dashDuration;
        Vector3 dashDirectionWhenStandStill = (transform.forward * dashSpeedWhenStandStill);

        if(_rb.velocity == Vector3.zero)
        {
            while(elapsedDashTime < dashDuration)
            {
                _rb.AddForce(dashDirectionWhenStandStill * Time.fixedDeltaTime, ForceMode.VelocityChange); 
                elapsedDashTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (elapsedDashTime < dashDuration)
            {
                _rb.AddForce(dashDirection * (dashForce / dashDuration) * Time.fixedDeltaTime, ForceMode.VelocityChange);
                elapsedDashTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        dashCancel?.Invoke();
        dashCoroutine = null;
        isRecovering = false;
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


    private IEnumerator Parry()
    {
        canParry = false;

        OnParryStart?.Invoke();

        float elapsedInvulnerableTime = invulnerableTimerOrg;
        float elapsedStandStillTime = standStillTimerOrg;

        Debug.Log("Parrying");

        while (elapsedInvulnerableTime >= 0)
        {
            elapsedInvulnerableTime -= Time.deltaTime;  // Use Time.deltaTime for frame-based time tracking

            Collider[] colliders = Physics.OverlapBox(parryBoxRefPoint.position, parryBoxSize / 2, transform.rotation, bulletLayerMask);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    GameObject actualBullet = colliders[i].transform.parent.gameObject; // I had to access the parent for some reason...

                    EnemyProjectile bulletScript = actualBullet.GetComponent<EnemyProjectile>();

                    if (bulletScript != null)
                    {
                        bulletScript.ReflectBulletReverse();
                    }
                    else
                    {
                        Debug.Log("Something wrong with calling the script from bulletScript");
                    }
                }
            }

            if (immobileCoroutine == null)
                immobileCoroutine = StartCoroutine(StandStill());

            yield return null;  
        }

    }

    private IEnumerator StandStill()
    {
        _rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(standStillTimerOrg);

        OnParryStop?.Invoke();

        canParry = true;

        parryCoroutine = null; // Gotta make sure the coroutine is null to match the condition above! And also, cleans up each coroutine after each use! ^^
        immobileCoroutine = null;
    }

    private void LookAtMousePosition() //Look at player mouse position
    {
        if(isAttacking)
        {
            return;
        }
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
    
    public void Parry(InputAction.CallbackContext context)
    {

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
