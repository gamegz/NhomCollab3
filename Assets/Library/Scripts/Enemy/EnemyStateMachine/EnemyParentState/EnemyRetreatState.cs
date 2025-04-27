using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.statemachine;


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
            
        }
    }
}

