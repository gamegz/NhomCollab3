using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyStateAttackSwordStrike : EnemyAttackState
    {
        [SerializeField] private int attackRange;
        [Range(2,4)]
        [SerializeField] private int strikeTimes;
        [SerializeField] private float innitTime = 1;
        [SerializeField] private float timeBetweenStrike = 2;
        private BossEnemyBase bossEnemy;

        [SerializeField] private float attackDashDistance = 20;
        [SerializeField] private float attackTime = 0.5f;

        [SerializeField] private float attackChaseSpeed;

        private float timeBetweenStrikeCount;
        private float strikeTimeCount;
        private float innitTimeCount;
        private bool finishAttack;

        private Coroutine attackDashCoroutine;

        public EnemyStateAttackSwordStrike(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            bossEnemy = enemy;
        }

        public override void SetUpState(EnemyBase enemy, EnemyStateMachine enemyStateMachine)
        {
            bossEnemy = (BossEnemyBase)enemy;
            base.SetUpState(enemy, enemyStateMachine);
        }

        public override void EnterState()
        {            
            base.EnterState();
            finishAttack = false;
            innitTimeCount = innitTime;
            strikeTimes = Random.Range(2, 4);
            strikeTimeCount = strikeTimes;
            timeBetweenStrikeCount = 0;
            bossEnemy.currentSpeed = attackChaseSpeed;
        }
       
        public override void FixedUpdateS()
        {
            base.FixedUpdateS();
        }

        public override void UpdateState()
        {            
            base.UpdateState();
            innitTimeCount -= Time.deltaTime;
            if (innitTimeCount > 0) { return; }


            if (finishAttack)
            {
                int randNum = Random.Range(1, 4);
                switch (randNum)
                {
                    case 1:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon1);
                        break;
                    case 2:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon2);
                        break;
                    case 3:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackAOE1);
                        break;
                }
            }


            if (bossEnemy.GetDistanceToPLayerIgnoreY() > attackRange)
            {
                _enemyNavMeshAgent.SetDestination(bossEnemy.playerRef.transform.position);
                timeBetweenStrikeCount -= Time.deltaTime;
                bossEnemy.canTurn = true;
            }
            else
            {
                if (strikeTimeCount <= 0)
                {
                    finishAttack = true;
                    return;
                }

                timeBetweenStrikeCount -= Time.deltaTime;
                if (timeBetweenStrikeCount > 0) { return; } //Attack
                bossEnemy.canTurn = false;
                attackDashCoroutine = StartCoroutine(bossEnemy.Dash(bossEnemy.GetDirectionToPlayer(), attackDashDistance, attackTime));
                bossEnemy.InnitAttackCollider(0.2f);
                strikeTimeCount--;
                timeBetweenStrikeCount = timeBetweenStrike;
            }





        }

        public override void ExitState()
        {
            bossEnemy.canTurn = true;
            base.ExitState();
        }

        //IEnumerator InnitAttackStrike()
        //{
        //    for(int i = 0; i < strikeTimes; i++)
        //    {
        //        bossEnemy.canTurn = false;
        //        bossEnemy.InnitDash(bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, bossEnemy.playerRef.transform.position), attackDashDistance, attackTime);
        //        bossEnemy.InnitAttackCollider(0.2f);
        //        yield return new WaitForSeconds(timeBetweenStrike);
        //    }
        //}
    }
}

