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

            //Debug.Log(_currentRoamTimeCountDown);

            if (_currentRoamDelayCountDown > 0) //Roam count down to reroam
            {
                if (_enemy.GetDestinationCompleteStatus())
                {
                    RoamRandomStandAndMove();

                }
                _currentRoamDelayCountDown -= Time.deltaTime;
            }
            else //end countdown, roam
            {

                RoamRandomStandAndMove();
            }

            if(_currentRoamTransitTime > 0)
            {
                _currentRoamTransitTime -= Time.deltaTime;


                if(_currentRoamTransitTime <= 0 && _enemy.isTokenOwner)
                {
                    _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
                }
            }

            

            //if (_enemy.isTargetInAttackRange)
            //{
            //    _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
            //}

            switch (_enemy.isTokenOwner)
            {
                case true:
                    if (_enemy.isTargetInChaseRange)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
                    }

                    //if (_enemy.isTargetInAttackRange)
                    //{
                    //    _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
                    //}
                    break;
                case false:
                    break;
            }


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
            _roamLocation = _enemy.GetRandomNavmeshLocation();
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

