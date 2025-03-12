using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Enemy.statemachine.States;

public class BossEnemyBase : EnemyBase ///This is getting messy af
{
    [HideInInspector] public EnemyAttackState enemyAttackMelee1;
    [HideInInspector] public EnemyAttackState enemyAttackMelee2;
    [HideInInspector] public EnemyAttackState enemyAttackAOE1;
    [HideInInspector] public EnemyAttackState enemyAttackSummon1;
    [HideInInspector] public EnemyAttackState enemyAttackSummon2;
    [HideInInspector] public EnemyAttackState enemyAttackRanged1;
    [HideInInspector] public EnemyAttackState enemyAttackUltimate1;

    [SerializeField] private GameObject handObj;

    public override void Awake()
    {
        base.Awake();
    }

    public override void UpdateLogic()
    {
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
        enemyRoamState = new EnemyStateBossRoam(this, _stateMachine);
        enemyChaseState = new EnemyStateChase(this, _stateMachine);
        enemyAttackState = new EnemyStateAttackSwingNormal(this, _stateMachine);
        enemyRetreatState = new EnemyStateRetreat(this, _stateMachine);
        enemyFollowState = new EnemyStateFollow(this, _stateMachine);
        _stateMachine.SetStartState(enemyRoamState);
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
