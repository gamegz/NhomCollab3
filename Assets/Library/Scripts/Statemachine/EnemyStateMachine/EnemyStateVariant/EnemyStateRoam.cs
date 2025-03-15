using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateRoam : EnemyRoamState
    {
        private Vector3 _roamLocation;

        private float _currentRoamDelayCountDown;
        private float _currentRoamTransitTime;

        public EnemyStateRoam(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            _currentRoamTransitTime = _enemy.roamDuration;
            RoamLocationRandom();
        }


        public override void UpdateState()
        {
            _enemy.UpdateLogicByPlayerDistance();

            if (_currentRoamDelayCountDown > 0) 
            {
                if (_enemy.GetDestinationCompleteStatus()) { RoamRandomStandAndMove(); }
                _currentRoamDelayCountDown -= Time.deltaTime;
                if(_currentRoamDelayCountDown <= 0)
                {
                    RoamRandomStandAndMove();
                }
            }//Reroam if reach destination or countdown ends


            if(_currentRoamTransitTime > 0) //Try transtioning with count down
            {
                _currentRoamTransitTime -= Time.deltaTime;
            }
            else
            {
                switch (_enemy.isTokenOwner)
                {
                    case true:
                        if (!_enemy.canAttack)
                        {
                            
                            if (_enemy.distanceToPlayer > _enemy.followDistance)
                            {
                                _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                                break;
                            }
                            if (_enemy.distanceToPlayer < _enemy.retreatDistance)
                            {
                                _ownerStateMachine.SwitchState(_enemy.enemyRetreatState);
                                break;
                            }
                            else
                            {
                                _currentRoamTransitTime = _enemy.roamDuration;
                            }

                            break;
                        }
                        else
                        {
                            if (_enemy.isTargetInAttackRange)
                            {
                                _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
                            }
                            else
                            {
                                _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
                            }
                        }


                        
                        break;

                    case false:
                        
                        if(_enemy.distanceToPlayer > _enemy.followDistance)
                        {
                            
                            _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                            break;
                        }
                        if(_enemy.distanceToPlayer < _enemy.retreatDistance)
                        {
                            _ownerStateMachine.SwitchState(_enemy.enemyRetreatState);
                            break;
                        }
                        _currentRoamTransitTime = _enemy.roamDuration;
                        break;
                }
                
            }

            

            //if (_enemy.isTargetInAttackRange)
            //{
            //    _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
            //}

          

        }

        public override void FixedUpdateS()
        {

        }

        public override void ExitState()
        {
            base.ExitState();
            _enemy.canMove = true;
        }

        public void RoamLocationRandom()
        {
            _enemy.canMove = true;
            _roamLocation = _enemy.GetRandomNavmeshLocationAroundSelf(_enemy.roamRadius);
            _enemyNavMeshAgent.SetDestination(_roamLocation);
            _currentRoamDelayCountDown = _enemy.roamCountDown;
        }

        public void RoamRandomStandAndMove() //W-what the hell man :(( - why?
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            int ranNum = Random.Range(0, 2);
            //Debug.Log(ranNum);
            switch (ranNum)
            {
                case 0:
                    _enemy.canMove = false;
                    _currentRoamDelayCountDown = _enemy.roamCountDown;
                    break;
                case 1:
                    RoamLocationRandom();
                    break;
                default:
                    RoamLocationRandom();
                    break;
            }
        }
    }
}

