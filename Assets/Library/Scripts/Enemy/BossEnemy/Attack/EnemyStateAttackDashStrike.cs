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
        [SerializeField] private float innitTime = 1.2f;
        [SerializeField] private float timeBetweenStrike = 2;
        [SerializeField] private float teleportRange = 15;
        private BossEnemyBase bossEnemy;

        [Tooltip("Time to travel a dashUnitDistance distance")]
        [SerializeField] private float dashTimePerUnit = 0.2f;
        [Tooltip("Distance traveled per dashTimeUnit time")]
        [SerializeField] private float dashUnitDistance = 10;

        [SerializeField] private float attackChaseSpeed;

        private float innitTimeCount;
        private bool finishAttack;
        private bool doneAttack;

        [Header("Black Hole")] 
        [SerializeField] private ParticleSystem blackHoleEffect;

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
            bossEnemy.canMove = true;
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
                
                // Effectttttttttttttttttttttttttttttttttttttt
                if (!_attackIndicatorPlayed)
                {
                    bossEnemy.PlayAttackIndicatorEffect();
                    _attackIndicatorPlayed = true;
                }

                if (bossEnemy.GetDistanceToPLayerIgnoreY() < retreatDistance)
                {
                    bossEnemy.UpdateLogicByPlayerDistance();
                    Vector3 retreatPos = _enemy.GetNavLocationByDirection(bossEnemy.transform.position,
                                                                          bossEnemy.transform.position - bossEnemy.playerRef.transform.position,
                                                                          4, 1);
                    bossEnemy.enemyNavAgent.SetDestination(retreatPos);
                }
                return;
            }


            if (finishAttack)
            {
                bossEnemy.attackMoveNum--;
                if (bossEnemy.attackMoveNum <= 0)
                {
                    _ownerStateMachine.SwitchState(bossEnemy.bossRoamState);
                    return;
                }

                //StopCoroutine(InnitAttack());
                float chanceNum = Random.value;
                int ranNum = Random.Range(1, 3);
                if (chanceNum <= 0.3f)
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
                    _ownerStateMachine.SwitchState(bossEnemy.enemyAttackAOE1);
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
            bossEnemy.canMove = true;
        }

        IEnumerator InnitAttack()
        {
            bossEnemy.currentSpeed = attackChaseSpeed;
            bossEnemy.enemyNavAgent.SetDestination(bossEnemy.playerRef.transform.position);
            RaycastHit hit;
            Vector3 playerPos;
            float distanceToPlayer;
            Vector3 directionToPlayer;
            float dashTimeToPlayer;
            bossEnemy.canMove = false;

            for (int i = 0; i < strikeTimes; i++)
            {
                playerPos = bossEnemy.playerRef.transform.position;
                Vector3 telePos = bossEnemy.GetNavMeshLocationAroundAPoint(playerPos, teleportRange);
                //float distancePlayerToTele = Vector3.Distance(telePos, playerPos);
                //if(Physics.Raycast(playerPos, bossEnemy.GetDirectionIgnoreY(playerPos, telePos), out hit ,distancePlayerToTele)){
                //    telePos = hit.transform.position;
                    
                //}
                
                PlayBlackHoleEffect();
                bossEnemy.enemyNavAgent.Warp(telePos);
                PlayBlackHoleEffect();

                directionToPlayer = bossEnemy.GetDirectionIgnoreY(bossEnemy.transform.position, playerPos);
                //bossEnemy.LookAtTarget(bossEnemy.playerRef.transform.position);
                yield return new WaitForSeconds(timeBetweenStrike);

                bossEnemy.canTurn = false;
                distanceToPlayer = bossEnemy.GetDistanceToPLayerIgnoreY() + 10;
                dashTimeToPlayer = (distanceToPlayer / dashUnitDistance) * dashTimePerUnit;
                StartCoroutine(bossEnemy.Dash(bossEnemy.transform.forward, distanceToPlayer, dashTimeToPlayer));
                bossEnemy.InnitAttackCollider(dashTimeToPlayer);
                yield return new WaitForSeconds(dashTimeToPlayer + 1);
            }

            finishAttack = true;
        }
        
        
        
        
        #region Effects

        private void PlayBlackHoleEffect()
        {
            blackHoleEffect.Play(); 
        }
        #endregion
    }
}

