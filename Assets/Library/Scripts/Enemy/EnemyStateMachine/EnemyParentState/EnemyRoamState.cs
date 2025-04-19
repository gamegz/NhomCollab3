using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyRoamState : BaseEnemyState
    {
        public EnemyRoamState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            
        }
        

        public override void EnterState()
        {
            _enemy.currentSpeed = _enemy.roamSpeed;
            _enemy.currentState = EnemyBase.EnemyState.Roam;
        }
        
        
        public override void UpdateState()
        {
            
        }

        public override void FixedUpdateS()
        {

        }

        public override void ExitState()
        {
            _enemy.currentSpeed = _enemy.roamSpeed;
        }      

        
    }
}

