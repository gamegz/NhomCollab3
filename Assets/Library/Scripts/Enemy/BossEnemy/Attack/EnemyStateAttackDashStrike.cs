using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    //Try to confuse player then dash toward them for a strike
    public class EnemyStateAttackDashStrike : EnemyAttackState
    {
        [SerializeField] private float retreatDistance;
        [Range(1,3)]
        [SerializeField] private int strikeTimes;
        [SerializeField] private float innitTime = 2;
        [SerializeField] private float timeBetweenStrike = 2;
        [SerializeField] private float attackRange = 5; //If too close then can attack rightaway
        private BossEnemyBase bossEnemy;

        [Tooltip("Time to travel a dashUnitDistance distance")]
        [SerializeField] private float dashTimePerUnit = 0.2f;
        [Tooltip("Distance traveled per dashTimeUnit time")]
        [SerializeField] private float dashUnitDistance = 10;

        [SerializeField] private float attackChaseSpeed;

        private Coroutine dashCoroutine;
        private float innitTimeCount;
        private bool finishAttack;
        private bool doneAttack;

        public EnemyStateAttackDashStrike(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
            doneAttack = false;
            innitTimeCount = innitTime;
            bossEnemy.currentSpeed = attackChaseSpeed;
        }
       
        public override void FixedUpdateS()
        {           
            base.FixedUpdateS();
        }

        public override void UpdateState()
        {            
            base.UpdateState();
            if (innitTimeCount > 0)
            {
                innitTimeCount -= Time.deltaTime;
                if (bossEnemy.GetDistanceToPLayerIgnoreY() < retreatDistance)
                {
                    bossEnemy.UpdateLogicByPlayerDistance();
                    Vector3 retreatPos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                          _enemy.transform.position - _enemy.playerRef.transform.position,
                                                                          4, 1);
                    bossEnemy.enemyNavAgent.SetDestination(retreatPos);
                }
                return;
            }
            bossEnemy.enemyNavAgent.SetDestination(bossEnemy.playerRef.transform.position);


            if (finishAttack)
            {
                int randNum = Random.Range(1, 3);
                switch (randNum)
                {
                    case 1:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon1);
                        break;
                    case 2:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon1);
                        break;
                }
            }


            if (!doneAttack)
            {
                StartCoroutine(InnitAttack());
                doneAttack = true;
            }

        }

        public override void ExitState()
        {
            base.ExitState();
        }

        IEnumerator InnitAttack()
        {
            bossEnemy.enemyNavAgent.SetDestination(bossEnemy.playerRef.transform.position);
            float distanceToPlayer = bossEnemy.GetDistanceToPLayerIgnoreY();
            Vector3 directionToPlayer = bossEnemy.GetDirectionToPlayer();
            float dashTimeToPlayer = (distanceToPlayer / dashUnitDistance) * dashTimePerUnit;

            for (int i = 0; i < strikeTimes; i++)
            {
                dashCoroutine = StartCoroutine(bossEnemy.Dash(directionToPlayer, distanceToPlayer, dashTimeToPlayer));
                if (dashCoroutine != null) { yield return null; }
                yield return new WaitForSeconds(timeBetweenStrike);
                bossEnemy.InnitAttackCollider(0.2f);
                
            }

            finishAttack = true;
        }
    }
}

