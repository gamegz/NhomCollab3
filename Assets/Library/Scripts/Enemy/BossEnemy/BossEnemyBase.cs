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

    public override void Awake()
    {
        base.Awake();
    }

    public override void UpdateLogic()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    InnitDash(GetDirectionToPlayer(), 30, 1f);
        //}
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
        enemyRoamState = new EnemyStateRoam(this, _stateMachine);
        enemyChaseState = new EnemyStateChase(this, _stateMachine);
        enemyAttackState = new EnemyStateAttackBoss(this, _stateMachine);

        enemyAttackMelee1.SetUpState(this, _stateMachine);
        enemyAttackMelee2.SetUpState(this, _stateMachine);
        //enemyAttackAOE1;
        enemyAttackSummon1.SetUpState(this, _stateMachine);
        //enemyAttackSummon2;
        //enemyAttackRanged1;
        //enemyAttackUltimate1;

        enemyRetreatState = new EnemyStateRetreat(this, _stateMachine);
        enemyFollowState = new EnemyStateFollow(this, _stateMachine);
        _stateMachine.SetStartState(enemyAttackSummon1);
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
