using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine.State;


namespace Enemy.statemachine
{
    public class BaseEnemyState : StateMachineBaseState
    {
        protected EnemyBase _enemyAgent;
        protected EnemyStateMachine _ownerStateMachine;

        public BaseEnemyState(EnemyBase enemy, EnemyStateMachine enemyStateMachine)
        {
            _enemyAgent = enemy;
            _ownerStateMachine = enemyStateMachine;
        }

        public override void EnterState()
        {
        
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
    }

}
