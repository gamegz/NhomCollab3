using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyStateAttackSummonSkull : EnemyAttackState
    {
        [SerializeField] private GameObject summonObj;
        [SerializeField] private float retreatDistance = 8;
        [SerializeField] private float retreatTime = 1;
        [SerializeField] private List<Transform> skullSpawnLocations;
        [SerializeField] private float skullSpawnDelay = 0.25f;
        private BossEnemyBase bossEnemy;

        private float retreatTimeCount;
        private bool finishAttack;
        private bool doAttack;

        public EnemyStateAttackSummonSkull(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
            retreatTimeCount = retreatTime;
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
            if (retreatTimeCount > 0)
            {
                retreatTimeCount -= Time.deltaTime;
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

            if (!doAttack)
            {
                // Effecttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt
                if (!_attackIndicatorPlayed)
                {
                    bossEnemy.PlayAttackIndicatorSound();
                    bossEnemy.PlayAttackIndicatorEffect();
                    _attackIndicatorPlayed = true;
                }

                StartCoroutine(SpawnSkullWithDelay());
                doAttack = true;
            };
            
            


            
            if (finishAttack)
            {
                bossEnemy.attackMoveNum--;
                if (bossEnemy.attackMoveNum <= 0)
                {
                    _ownerStateMachine.SwitchState(bossEnemy.bossRoamState);
                    return;
                }

                //Transition
                float chanceNum = Random.value;
                //int ranNum = Random.Range(1, 3);
                if (chanceNum <= 0.35f)
                {
                    _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee1);

                }
                else
                {
                    _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee2);
                }
            }
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        IEnumerator SpawnSkullWithDelay()
        {           
            for (int i = 0; i < skullSpawnLocations.Count; i++)
            {
                //Vector3 dirToPlayer = bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, bossEnemy.playerRef.transform.position);
                GameObject projectile = Instantiate(summonObj, skullSpawnLocations[i].position, Quaternion.Euler(bossEnemy.GetDirectionToPlayer()));
                if (projectile.TryGetComponent<ProjectileFollow>(out ProjectileFollow enemyProjectile))
                {
                    enemyProjectile.SetUp(bossEnemy.GetDirectionToPlayer(), this.gameObject, bossEnemy.playerRef.transform);
                }
                yield return new WaitForSeconds(skullSpawnDelay);
            }

            finishAttack = true;
        }
    }
}

