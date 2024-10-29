using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateAttackCharge : EnemyAttackState
    {

        public EnemyStateAttackCharge(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void EnterState()
        {
            
        }

      
        public override void FixedUpdateS()
        {
            base.FixedUpdateS();
        }

        public override void UpdateState()
        {
            base.FixedUpdateS();
            _enemy.UpdateLogicByPlayerDistance();

            if (_enemy.isTargetInAttackRange)
            {

            }
            else
            {
                _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
            }
        }
        
        public override void ExitState()
        {
            
        }
    }
}

