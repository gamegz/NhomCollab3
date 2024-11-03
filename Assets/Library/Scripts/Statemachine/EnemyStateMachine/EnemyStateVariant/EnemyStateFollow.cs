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



                    //if (_enemy.GetDistanceToPLayer() > _enemy.followDistance)
                    //{
                    //    _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);
                    //}

                    if (_enemy.canAttack)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
                        break;
                    }
                    else //Cant attack, retreat when too close, follow when needed, otherwise go random 
                    {
                        if (_enemy.GetDistanceToPLayer() < _enemy.retreatDistance)
                        {
                            _ownerStateMachine.SwitchState(_enemy.enemyRetreatState);
                            break;
                        }

                        if (_enemy.GetDistanceToPLayer() > _enemy.followDistance)
                        {
                            _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);
                            break;
                        }

                        _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
                    }

                    break;
                case false:
                    //Stop following and transit
                    if (_enemy.GetDistanceToPLayer() < _enemy.retreatDistance)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyRetreatState);
                        break;
                    }

                    if (_enemy.GetDistanceToPLayer() > _enemy.followDistance)
                    {
                        _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);
                        break;
                    }

                    _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
                    break;
            }
        }          
        
        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

