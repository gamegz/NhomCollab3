using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyStateAttackSummonDemonHand : EnemyAttackState
    {
        [SerializeField] private GameObject summonObj;
        [SerializeField] private float retreatDistance = 8;
        [SerializeField] private float innitTime = 1;
        [SerializeField] private Transform spawnLocation;
        private BossEnemyBase bossEnemy;

        private float attackTime;
        private float innitTimeCount;
        private bool finishAttack;
        private bool doAttack;

        public EnemyStateAttackSummonDemonHand(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
            doAttack = false;
            finishAttack = false;
            innitTimeCount = innitTime;
            if(bossEnemy.GetDistanceToPLayerIgnoreY() < retreatDistance)
            {
                bossEnemy.InnitDash(bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, bossEnemy.playerRef.transform.position), 25, 1f);
            }
        }
       
        public override void FixedUpdateS()
        {                      
            base.FixedUpdateS();
        }

        public override void UpdateState()
        {            
            base.UpdateState();

            if (finishAttack)
            {
                int randNum = Random.Range(1, 4);
                switch (randNum)
                {
                    case 1:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee1);
                        break;
                    case 2:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee2);
                        break;
                    case 3:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackRanged1);
                        break;
                }
            }

            if (innitTimeCount > 0)
            {
                innitTimeCount -= Time.deltaTime;               
                return;
            }

            if (bossEnemy.GetDistanceToPLayerIgnoreY() < retreatDistance)
            {
                bossEnemy.UpdateLogicByPlayerDistance();
                Vector3 retreatPos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                      _enemy.transform.position - _enemy.playerRef.transform.position,
                                                                      4, 1);
                bossEnemy.enemyNavAgent.SetDestination(retreatPos);

            }

            if (!doAttack)
            {
                Vector3 dirToPlayer = bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, bossEnemy.playerRef.transform.position);
                GameObject projectile = Instantiate(summonObj, spawnLocation.position, Quaternion.Euler(dirToPlayer));
                if (projectile.TryGetComponent<SummonHandProjectile>(out SummonHandProjectile enemyProjectile))
                {
                    attackTime = enemyProjectile.LifeTime;
                    enemyProjectile.SetUp(dirToPlayer, this.gameObject, bossEnemy.playerRef.transform);
                }
                doAttack = true;
            };

            attackTime -= Time.deltaTime;
            if(attackTime < 0)
            {
                finishAttack = true;
            }                       
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

