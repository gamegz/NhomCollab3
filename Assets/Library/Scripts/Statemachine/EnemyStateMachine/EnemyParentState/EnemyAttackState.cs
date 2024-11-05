using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyAttackState : BaseEnemyState
    {
        public EnemyAttackState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void EnterState()
        {
            _enemy.currentSpeed = _enemy.chaseSpeed;
            _enemy.currentState = EnemyBase.EnemyState.Attack;
        }

        public override void ExitState()
        {
            
        }

        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            
        }

        protected void InnitAttackCollider()
        {
            _enemy.attackCollider.enabled = true;
        }
    }
}

