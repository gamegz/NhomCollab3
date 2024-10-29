using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyRoamState : BaseEnemyState
    {
        private EnemyChaseState _enemyChaseState;

        private Vector3 _roamLocation;

        float _currentRoamTimeCountDown;


        public EnemyRoamState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

            if(enemy.TryGetComponent(out EnemyChaseState enemyChaseState))
            {
                _enemyChaseState = enemyChaseState;
            }

            
        }
        

        public override void EnterState()
        {
            ChangeRoamLocation();
        }

        
        
        public override void UpdateState()
        {
            _enemy.UpdateLogicByPlayerDistance();

            //Debug.Log(_currentRoamTimeCountDown);

            if (_currentRoamTimeCountDown > 0) {
                if (_enemy.GetDestinationCompleteStatus())
                {
                    ChangeRoamLocation();

                }
                _currentRoamTimeCountDown -= Time.deltaTime;
            }
            else {
                ChangeRoamLocation();
            }
            

            if (_enemy.isTargetInChaseRange)
            {
                _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
            }
        }

        public override void FixedUpdateS()
        {

        }

        public override void ExitState()
        {
            
        }

        public void ChangeRoamLocation()
        {
            _currentRoamTimeCountDown = _enemy.roamDelay;
            _roamLocation = _enemy.GetRandomNavmeshLocation();
            _enemyNavMeshAgent.SetDestination(_roamLocation);
        }

        
    }
}

