using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateChaseDash1 : EnemyChaseState
    {
        private float _chaseToAttackTimeCount;

        private float dashDelay = 3;
        private float dashDelayCount;

        private float dashChaseStoppingDistance = 9f;

        private bool dashRight;
        private float dashDistance = 13;
        private float dashTime = 0.5f;


        public EnemyStateChaseDash1(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            dashDelayCount = dashDelay;

            _chaseToAttackTimeCount = _enemy.chaseToAttackTransitTime;
            
        }

      
        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            _enemy.UpdateLogicByPlayerDistance();

            _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);


            DashUpdateChase();


            switch (_enemy.isTokenOwner)
            {
                case true:

                    

                    if (_enemy.canAttack)
                    {
                        _chaseToAttackTimeCount -= Time.deltaTime;
                        if(_chaseToAttackTimeCount <= 0)
                        {
                            _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
                            break;
                        }

                        if (_enemy.isTargetInAttackRange)
                        {
                            _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
                            break;
                        }
                    }
                    else
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                    }

                    break;
                case false:
                    _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
                    break;
            }

            
        }

        private void DashUpdateChase()
        {
            _enemy.canTurn = true;
            if (_enemy.isDashing) { return; }

            if (dashDelayCount > 0)
            {
                dashDelayCount -= Time.deltaTime;

            }
            else
            {
                Random.InitState(System.DateTime.Now.Millisecond);

                if (_enemy.DashDistance < dashChaseStoppingDistance)
                {                   
                    _enemy.InnitDash(_enemy.GetPerpendicularDirectionToPLayerTarget(dashRight), dashDistance, dashTime);
                    dashRight = !dashRight;


                    dashDelayCount = Random.Range(0.8f, 1.65f);
                }
                else
                {
                    _enemy.InnitDash(_enemy.GetOffSetDirection(_enemy.GetDirectionToPlayer(), Random.Range(20, 45)), dashDistance, 0.2f);
                    dashDelayCount = Random.Range(0.3f, 1f);
                }
            }
        }
        
        public override void ExitState()
        {
            base.ExitState();
            dashDelayCount = Random.Range(1, 2f);
        }
    }
}

