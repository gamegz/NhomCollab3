using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateRetreat : EnemyRetreatState
    {
        private float _runDistance = 5;
        private float _runPosOffSet = 1;

        public EnemyStateRetreat(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
            _enemy.UpdateLogicByPlayerDistance();
            Vector3 retreatPos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                  _enemy.transform.position - _enemy.playerRef.transform.position,
                                                                  _runDistance, _runPosOffSet);
            _enemy.enemyNavAgent.SetDestination(retreatPos);

            if (_enemy.distanceToPlayer > _enemy.retreatDistance)
            {
                _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
            }

        }
        
        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

