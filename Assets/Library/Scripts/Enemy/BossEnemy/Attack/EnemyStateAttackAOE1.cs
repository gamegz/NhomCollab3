using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyStateAttackAOE1 : EnemyAttackState
    {
        [SerializeField] private ParticleSystem ExplodeEffect;
        [SerializeField] private float innitTime = 2;
        //[SerializeField] private Transform explodeLocation;
        [SerializeField] private float explodeRange = 4;
        [SerializeField] private int explodeDamage = 1;
        [SerializeField] private float exitStateTime;
        private BossEnemyBase bossEnemy;

        //private ParticleSystem exploEffect;
        private float exitStateTimeCount;
        private float innitTimeCount;
        private bool finishAttack;
        private bool doAttack;

        public EnemyStateAttackAOE1(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
            exitStateTimeCount = exitStateTime;

            bossEnemy.InnitDash(bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, bossEnemy.playerRef.transform.position), 8, 0.2f);
            bossEnemy.canTurn = false;
            bossEnemy.canMove = false;
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
                exitStateTimeCount -= Time.deltaTime;
                if (exitStateTimeCount > 0) { return; }

                bossEnemy.attackMoveNum--;
                if (bossEnemy.attackMoveNum <= 0)
                {
                    _ownerStateMachine.SwitchState(bossEnemy.bossRoamState);
                    return;
                }

                float chanceNum = Random.value;
                int ranNum = Random.Range(1, 3);
                if (chanceNum <= 0.2f)
                {
                    switch (ranNum)
                    {
                        case 1:
                            _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon1);
                            break;
                        case 2:
                            _ownerStateMachine.SwitchState(bossEnemy.enemyAttackRanged1);
                            break;
                    }

                }
                else
                {
                    _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee1);
                }
            }

            if (innitTimeCount > 0)
            {
                innitTimeCount -= Time.deltaTime;               
                
                // Effectttttttttttttttttttttttttttttttttttttttttttttt
                if (!_attackIndicatorPlayed)
                {
                    bossEnemy.PlayAttackIndicatorSound();
                    bossEnemy.PlayAttackIndicatorEffect();
                    _attackIndicatorPlayed = true;
                }

                return;
            }

            if (!doAttack)
            {
                //exploEffect = Instantiate(ExplodeEffect, explodeLocation.transform.position, Quaternion.identity);
                //exploEffect.Play();
                //Destroy(exploEffect, exploEffect.GetComponent<ParticleSystem>().main.duration);
                //RaycastHit hit;
                //if(Physics.SphereCast(explodeLocation.transform.position, explodeRange, Vector3.zero, out hit, 0, bossEnemy.layerData.hostileTargetLayer))
                //{
                //    hit.transform.gameObject.GetComponent<IDamageable>().TakeDamage(explodeDamage);
                //}

                ExplodeEffect.Play();
                bossEnemy.AOEAttackSound.Play();
                Collider[] hitColliders = Physics.OverlapSphere(ExplodeEffect.transform.position, explodeRange, bossEnemy.layerData.hostileTargetLayer);
                foreach (var hitCollider in hitColliders) { 
                    hitCollider.transform.gameObject.GetComponent<IDamageable>().TakeDamage(explodeDamage);
                    
                }
                doAttack = true;
                finishAttack = true;
            };                                 
        }

        public override void ExitState()
        {
            base.ExitState();
            bossEnemy.canTurn = true;
            bossEnemy.canMove = true;
        }
    }
}

