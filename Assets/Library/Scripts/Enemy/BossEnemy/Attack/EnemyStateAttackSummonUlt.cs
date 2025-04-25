using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.statemachine.States
{
    public class EnemyStateAttackSummonUlt : EnemyAttackState
    {
        [SerializeField] private GameObject summonObj;
        [SerializeField] private float innitTime = 2;
        [SerializeField] private float attackDelay = 3;
        [SerializeField] private int spawnNum = 30;
        [SerializeField] private int projectilePerSpawnNum = 2;
        [SerializeField] private float spawnDelay = 0.1f;
        [SerializeField] private float exitStateDelay = 2f;
        [SerializeField] private Transform shootLocation;
        [SerializeField] private Transform teleportLocation;
        private BossEnemyBase bossEnemy;

        //private float innitTimeCount;
        private bool finishAttack;
        private bool doAttack;

        public EnemyStateAttackSummonUlt(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
            //innitTimeCount = innitTime;
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
                _ownerStateMachine.SwitchState(bossEnemy.bossRoamState);
            }

            //if (innitTimeCount > 0)
            //{
            //    innitTimeCount -= Time.deltaTime;               
            //   return;
            //}


            if (!doAttack)
            {
                StartCoroutine(InnitUltAttack());
                doAttack = true;
            };                      
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        IEnumerator InnitUltAttack()
        {
            // Effectttttttttttttttttttttttttttttttttttttt
            if (!_attackIndicatorPlayed)
            {
                bossEnemy.PlayAttackIndicatorEffect();
                _attackIndicatorPlayed = true;
            }
            
            yield return new WaitForSeconds(innitTime);
            //teleport
            Vector3 telepos = bossEnemy.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(teleportLocation.position, out hit, 1, 1))
            {
                telepos = hit.position;
            }
            bossEnemy.enemyNavAgent.Warp(telepos);

            yield return new WaitForSeconds(attackDelay);
            //Shoot a bunch of projectile
            for (int i = 0; i < spawnNum; i++)
            {
                int clusterCount = projectilePerSpawnNum;
                while (clusterCount > 0)
                {
                    Vector3 shootDir = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y).normalized;
                    GameObject projectile = Instantiate(summonObj, shootLocation.position, Quaternion.Euler(shootDir));
                    if (projectile.TryGetComponent<EnemyProjectile>(out EnemyProjectile enemyProjectile))
                    {
                        enemyProjectile.SetUp(shootDir, bossEnemy.gameObject);
                    }
                    clusterCount--;
                }
                

                yield return new WaitForSeconds(spawnDelay);
            }
            //Finishshooting - wait a bit
            yield return new WaitForSeconds(exitStateDelay);

            finishAttack = true;

        }


    }
}

