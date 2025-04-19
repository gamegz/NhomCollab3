using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyFollowState : BaseEnemyState
    {
        public EnemyFollowState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void EnterState()
        {
            _enemy.currentSpeed = _enemy.followSpeed;
            _enemy.currentState = EnemyBase.EnemyState.Follow;
        }

        public override void ExitState()
        {
            
        }

        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            _enemy.currentSpeed = _enemy.followSpeed;
        }
    }
}

