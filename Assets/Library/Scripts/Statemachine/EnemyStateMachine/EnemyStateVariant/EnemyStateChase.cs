using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateChase : EnemyChaseState
    {

        public EnemyStateChase(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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

            _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);

            if (_enemy.isTargetInAttackRange)
            {
                _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
            }
        }
        
        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

