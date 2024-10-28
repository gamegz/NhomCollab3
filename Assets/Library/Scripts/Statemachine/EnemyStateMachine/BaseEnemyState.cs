using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine.State;
using UnityEngine.AI;


namespace Enemy.statemachine
{
    //Handle switching state logic
    //Handle state logic
    public class BaseEnemyState : StateMachineBaseState
    {
        protected EnemyBase _enemy;
        protected NavMeshAgent _enemyNavMeshAgent;
        protected EnemyStateMachine _ownerStateMachine;

        public BaseEnemyState(EnemyBase enemy, EnemyStateMachine enemyStateMachine)
        {
            _enemy = enemy;
            _enemyNavMeshAgent = enemy.enemyNavAgent;
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
