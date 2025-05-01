using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Enemy;
using Enemy.statemachine.States;
using Random = UnityEngine.Random;

public class BossEnemyBase : EnemyBase ///This is getting messy af
{
    public static Action OnBossDeath;
    
    [Header("ATTACK STATES")]
    public EnemyAttackState enemyAttackMelee1;
    public EnemyAttackState enemyAttackMelee2;
    public EnemyAttackState enemyAttackAOE1;
    public EnemyAttackState enemyAttackSummon1;
    public EnemyAttackState enemyAttackSummon2;
    public EnemyAttackState enemyAttackRanged1;
    public EnemyAttackState enemyAttackUltimate1;
    public EnemyAttackState bossAttackDefault;
    [HideInInspector] public int attackMoveNum = 3;
    [Space]
    public EnemyRoamState bossRoamState;
    [HideInInspector] public float roamTime = 2.8f;
    
    //public EnemyChaseState bossChase;
    //public EnemyRetreatState bossRetreat;
    //public EnemyFollowState bossFollow;

    public AudioSource bossWalkSound;
    public AudioSource lazerShootSound;
    public AudioSource teleportSound;
    public AudioSource AOEAttackSound;
    public AudioSource swordSlashSound;

    private PlayerInput playerInput;
    private int evadeChance = 18;

    public override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
    }

    protected override void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Attack.performed += OnPlayerTryAttack;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Attack.performed -= OnPlayerTryAttack;
        base.OnDisable();
    }

    public override void UpdateLogic()
    {
        if(this == null) {  return; }
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    //InnitDash(GetDirectionToPlayer(), 30, 1f);
        //    float distanceToPlayer = GetDistanceToPLayerIgnoreY() * 3;
        //    Vector3 directionToPlayer = GetDirectionIgnoreY(transform.position, playerRef.transform.position);
        //    float dashTimeToPlayer = (distanceToPlayer / 10) * 0.15f;
        //    InnitDash(directionToPlayer.normalized, distanceToPlayer, dashTimeToPlayer);
        //}

        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 telePos = GetNavMeshLocationAroundAPoint(playerRef.transform.position, 15);
            enemyNavAgent.Warp(telePos);

            //Vector3 dirToTarget = GetDirectionIgnoreY(transform.position, playerRef.transform.position);
            //Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            //transform.rotation = rotation;
        }
        base.UpdateLogic();
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void OnDeath()
    {
        OnBossDeath?.Invoke();
        base.OnDeath();
    }

    public override void SetUpStateMachine()
    {
        base.SetUpStateMachine();
        bossRoamState.SetUpState(this, _stateMachine);
        //bossChase.SetUpState(this, _stateMachine);
        //bossRetreat.SetUpState(this, _stateMachine);
        //bossFollow.SetUpState(this, _stateMachine);

        bossAttackDefault.SetUpState(this, _stateMachine);

        enemyAttackMelee1.SetUpState(this, _stateMachine);
        enemyAttackMelee2.SetUpState(this, _stateMachine);
        enemyAttackAOE1.SetUpState(this, _stateMachine);
        enemyAttackSummon1.SetUpState(this, _stateMachine);
        enemyAttackSummon2.SetUpState(this, _stateMachine);
        enemyAttackRanged1.SetUpState(this, _stateMachine);
        enemyAttackUltimate1.SetUpState(this, _stateMachine);

        _stateMachine.SetStartState(bossRoamState);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (currentHealth < ((maxHealth / 100) * 40))
        {
            evadeChance = 10;
        }

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    private void OnPlayerTryAttack(InputAction.CallbackContext context)
    {        
        int ranNum = Random.Range(0, evadeChance);
        if(ranNum == 1)
        {
            if(GetDistanceToPLayerIgnoreY() <= 4.3)
            {
                InnitDash(GetDirectionIgnoreY(playerRef.transform.position, transform.position), 13, 0.1f);
            }
        }
    }
}
