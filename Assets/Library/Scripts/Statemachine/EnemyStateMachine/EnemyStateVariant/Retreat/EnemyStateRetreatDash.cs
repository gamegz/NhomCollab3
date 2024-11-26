using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateRetreatDash : EnemyRetreatState
    {
        private float _runDistance = 5;
        private float _runPosOffSet = 1;
        private bool dashRight;
        private float dashDistance = 14;
        private float dashTime = 0.3f;

        private float retreatDashDelay = 2;
        private float retreatDashDelayCount;

        private bool dashRetreatOnce;

        public EnemyStateRetreatDash(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            dashRetreatOnce = false;
            retreatDashDelayCount = retreatDashDelay;
            _enemy.currentSpeed = _enemy.retreatSpeed;
            _enemy.InnitDash(_enemy.GetPerpendicularDirectionToPLayerTarget(dashRight), dashDistance, dashTime);
        }

      
        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            _enemy.UpdateLogicByPlayerDistance();
            Vector3 retreatPos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                  _enemy.transform.position - _enemy.playerRef.transform.position,
                                                                  _runDistance, _runPosOffSet);
            _enemy.enemyNavAgent.SetDestination(retreatPos);


            RetreatUpdateDash();

            switch (_enemy.isTokenOwner)
            {
                case true:
                    if(_enemy.isDashing) { return; }

                    if (!_enemy.canAttack)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                        break;
                    }

                    if (_enemy.isTargetInAttackRange)
                    {   
                        if(!_enemy.canAttack) { return; }
                        _ownerStateMachine.SwitchState(_enemy.enemyAttackState);                       
                    }
                    else
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyChaseState);                       
                    }
                    break;
                   
                case false:

                    if (_enemy.isDashing) { return; }
                    if (_enemy.distanceToPlayer < _enemy.maxRetreatDistance)
                    {                        
                        break;
                    }

                    //To far then follow otherwise roam
                    if (_enemy.distanceToPlayer > _enemy.followDistance)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                    }
                    else
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
                    }
                    break;

            }
        }
        
        public void RetreatUpdateDash()
        {
            if (dashRetreatOnce == false && !_enemy.isDashing)
            {
                _enemy.InnitDash(-_enemy.GetDirectionToPlayer(), dashDistance, dashTime);
                dashRetreatOnce = true;

            }

            if( dashRetreatOnce == false) { return; }
            if(_enemy.isDashing) { return; }

            retreatDashDelayCount -= Time.deltaTime;
            if(retreatDashDelayCount <= 0)
            {
                Random.InitState(System.DateTime.Now.Millisecond);
                int ranNum = Random.Range(0, 2);
                //Debug.Log(ranNum);
                switch (ranNum)
                {
                    case 0:
                        _enemy.InnitDash(-_enemy.GetDirectionToPlayer(), dashDistance, dashTime);
                        retreatDashDelayCount = Random.Range(0.5f, 1);
                        break;
                    case 1:
                        _enemy.InnitDash(_enemy.GetPerpendicularDirectionToPLayerTarget(dashRight), dashDistance, dashTime);
                        dashRight = !dashRight;
                        retreatDashDelayCount = Random.Range(0.4f, 0.8f);
                        break;
                    default:
                        _enemy.InnitDash(_enemy.GetPerpendicularDirectionToPLayerTarget(dashRight), dashDistance, dashTime);
                        dashRight = !dashRight;
                        retreatDashDelayCount = Random.Range(0.4f, 0.8f);
                        break;
                }


            }


        }

        public override void ExitState()
        {
            base.ExitState();
            dashRight = !dashRight;

        }
    }
}

