using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Enemy.statemachine.States;

public class BossEnemyBase : EnemyBase ///This is getting messy af
{
    [Header("ATTACK STATES")]
    public EnemyAttackState enemyAttackMelee1;
    public EnemyAttackState enemyAttackMelee2;
    public EnemyAttackState enemyAttackAOE1;
    public EnemyAttackState enemyAttackSummon1;
    public EnemyAttackState enemyAttackSummon2;
    public EnemyAttackState enemyAttackRanged1;
    public EnemyAttackState enemyAttackUltimate1;
    [Space]
    public EnemyRoamState bossRoam;
    public EnemyChaseState bossChase;
    public EnemyRetreatState bossRetreat;
    public EnemyFollowState bossFollow;

    public override void Awake()
    {
        base.Awake();
    }

    public override void UpdateLogic()
    {
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
        base.OnDeath();
    }

    public override void SetUpStateMachine()
    {
        base.SetUpStateMachine();
        //bossRoam.SetUpState(this, _stateMachine);
        //bossChase.SetUpState(this, _stateMachine);
        //bossRetreat.SetUpState(this, _stateMachine);
        //bossFollow.SetUpState(this, _stateMachine);

        enemyAttackMelee1.SetUpState(this, _stateMachine);
        enemyAttackMelee2.SetUpState(this, _stateMachine);
        enemyAttackAOE1.SetUpState(this, _stateMachine);
        enemyAttackSummon1.SetUpState(this, _stateMachine);
        enemyAttackSummon2.SetUpState(this, _stateMachine);
        enemyAttackRanged1.SetUpState(this, _stateMachine);
        //enemyAttackUltimate1.SetUpState(this, _stateMachine);

        _stateMachine.SetStartState(enemyAttackMelee1);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
