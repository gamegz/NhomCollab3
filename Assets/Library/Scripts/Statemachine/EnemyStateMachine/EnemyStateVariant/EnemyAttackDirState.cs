using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyAttackDirState : EnemyAttackState
    {
        private EnemyRoamState _enemyRoamState;

        public EnemyAttackDirState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void EnterState()
        {
            Debug.Log("asdasd");
            //base.EnterState();
            
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
                Debug.Log("asdasd");
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

