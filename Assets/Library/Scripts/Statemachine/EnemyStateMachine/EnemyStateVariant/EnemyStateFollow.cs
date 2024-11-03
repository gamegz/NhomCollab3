using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateFollow : EnemyFollowState
    {
        private Vector3 _followPoint;
        private float followRadius = 3;

        public EnemyStateFollow(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
        }

      
        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            //Kept following
            

            switch (_enemy.isTokenOwner)
            {
                case true:

                    _followPoint = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                            -_enemy.GetDirectionToPlayer(), //Dir from player to enemy
                                                            followRadius, 1);
                    _enemy.enemyNavAgent.SetDestination(_followPoint);

                    if (_enemy.canAttack)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
                    }

                    break;
                case false:
                    //Stop following and transit
                    if(_enemy.GetDistanceToPLayer() < _enemy.retreatDistance)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyRetreatState);
                    }
                    else
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
                    }
                    break;
            }
        }
        
        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

