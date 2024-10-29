using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateRoam : EnemyAttackState
    {

        public EnemyStateRoam(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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

        }
        
        public override void ExitState()
        {
            
        }
    }
}

