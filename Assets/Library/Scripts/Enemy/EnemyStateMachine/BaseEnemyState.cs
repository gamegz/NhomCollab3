using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine.State;
using UnityEngine.AI;


namespace Enemy.statemachine
{
    //Handle switching state logic
    //Handle state logic

    /*
    To create a new state:
        Inherit from this class
        Give coresponding Condition
    */

    public abstract class BaseEnemyState : StateMachineBaseState
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

        public virtual void SetUpState(EnemyBase enemy, EnemyStateMachine enemyStateMachine)
        {
            _enemy = enemy;
            _enemyNavMeshAgent = enemy.enemyNavAgent;
            _ownerStateMachine = enemyStateMachine;
        }

        public abstract override  void EnterState();
        public abstract override void FixedUpdateS();
        public abstract override void UpdateState();
        public abstract override void ExitState();
    }

}
