using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.statemachine;


namespace Enemy.statemachine.States
{
    public class EnemyChaseState : BaseEnemyState
    {

        public EnemyChaseState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            _enemy.currentSpeed = _enemy.chaseSpeed;
            _enemy.currentState = EnemyBase.EnemyState.Chase;
            if(_enemy.enemyAnimators != null)
                _enemy.enemyAnimators.SetTrigger("Move");
        }

        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            _enemy.currentSpeed = _enemy.chaseSpeed;
        }
    }
}

