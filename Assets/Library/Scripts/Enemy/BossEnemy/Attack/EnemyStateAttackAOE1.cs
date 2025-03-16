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
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon1);
                        break;
                }
            }

            if (innitTimeCount > 0)
            {
                innitTimeCount -= Time.deltaTime;               
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
                Collider[] hitColliders = Physics.OverlapSphere(ExplodeEffect.transform.position, explodeRange, bossEnemy.layerData.hostileTargetLayer);
                foreach (var hitCollider in hitColliders) { 
                    hitCollider.transform.gameObject.GetComponent<IDamageable>().TakeDamage(explodeDamage);
                    Debug.Log("HitPlayer");
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

