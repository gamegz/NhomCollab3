using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyStateAttackBossShoot : EnemyAttackState
    {
        [SerializeField] private ParticleSystem lazerEffect;
        [SerializeField] private float retreatDistance = 8;
        [SerializeField] private float innitTime = 1;       
        [SerializeField] private float changeDirTime = 2;
        [SerializeField] private float attackDelayTime = 0.8f;
        [SerializeField] private float exitStateTime = 1;
        [SerializeField] private int damage = 1;
        [SerializeField] private Transform shootLocation;
        private BossEnemyBase bossEnemy;

        //private ParticleSystem _lazerEffect;
        private float innitTimeCount;
        private bool finishAttack;
        private bool doAttack;

        public EnemyStateAttackBossShoot(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
                bossEnemy.InnitDash(-bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, bossEnemy.playerRef.transform.position), 10, 0.5f);
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
                bossEnemy.attackMoveNum--;
                if (bossEnemy.attackMoveNum <= 0)
                {
                    _ownerStateMachine.SwitchState(bossEnemy.bossRoamState);
                    return;
                }

                //Transition
                float chanceNum = Random.value;
                int ranNum = Random.Range(1, 3);
                if (chanceNum <= 0.35f)
                {
                    _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee1);

                }
                else
                {
                    switch (ranNum)
                    {
                        case 1:
                            _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon1);
                            break;
                        case 2:
                            _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon2);
                            break;
                    }
                }
            }

            if (innitTimeCount > 0)
            {
                innitTimeCount -= Time.deltaTime;               
                return;
            }

            //if (bossEnemy.GetDistanceToPLayerIgnoreY() < retreatDistance)
            //{
            //    bossEnemy.UpdateLogicByPlayerDistance();
            //    Vector3 retreatPos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
            //                                                          _enemy.transform.position - _enemy.playerRef.transform.position,
            //                                                          4, 1);
            //    bossEnemy.enemyNavAgent.SetDestination(retreatPos);

            //}

            if (!doAttack)
            {
                StartCoroutine(InnitBossShootAttack());               
                doAttack = true;
            };                       
        }

        public override void ExitState()
        {
            base.ExitState();
            bossEnemy.canTurn = true;
        }

        IEnumerator InnitBossShootAttack()
        {
            bossEnemy.canTurn = true;
            yield return new WaitForSeconds(changeDirTime);
            bossEnemy.canTurn = false;
            yield return new WaitForSeconds(attackDelayTime);
            float distanceToPlayer = bossEnemy.GetDistanceToPLayerIgnoreY();
            Vector3 dirToPlayer = bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, bossEnemy.playerRef.transform.position);
            RaycastHit[] hit;
            lazerEffect.Play();
            hit = Physics.BoxCastAll(shootLocation.position, new Vector3(1, 9, 1), bossEnemy.transform.forward, Quaternion.Euler(bossEnemy.transform.forward), distanceToPlayer, bossEnemy.layerData.hostileTargetLayer);
            foreach(RaycastHit rayHit in hit)
            {
                if(rayHit.transform.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)){
                    damageable.TakeDamage(damage);
                }
            }
            bossEnemy.canTurn = false;
            yield return new WaitForSeconds(exitStateTime);            
            finishAttack = true;
        }
    }
}

