using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.statemachine;
using Enemy.variant;


namespace Enemy.statemachine.States
{
    public class EnemyRetreatState : BaseEnemyState
    {
        public EnemyRetreatState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            _enemy.currentState = EnemyBase.EnemyState.Retreat;
            _enemy.currentSpeed = _enemy.followSpeed;
            if(_enemy.enemyAnimators != null)
            {
                if(_enemy is not EnemyUnderBoss)
                    _enemy.enemyAnimators.SetTrigger("Move");
                
            }

            if (_enemy is EnemyUnderBoss)
            {
                _enemy.enemyAnimators.SetTrigger("MoveBack");
            }
            
        }

        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}

