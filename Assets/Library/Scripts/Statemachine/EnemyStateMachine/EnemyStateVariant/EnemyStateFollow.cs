using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateFollow : EnemyFollowState
    {

        public EnemyStateFollow(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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

        }
        
        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

