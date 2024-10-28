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
            _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);
        }

        public override void FixedUpdateS()
        {
            _enemy.UpdateLogicByPlayerDistance();

            _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);

            if (_enemy.isTargetInAttackRange)
            {
                _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
            }
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }
}

