using Enemy;
using Enemy.statemachine.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.variant
{
    public class EnemyThug : EnemyBase
    {
        private EnemyRoamState _enemyRoamState;
        private EnemyChaseState _enemyChaseState;
        private EnemyAttackDirState _enemyAttackDirState;

        public override void Awake()
        {
            base.Awake();
        }

        public override void SetUpStateMachine()
        {
            base.SetUpStateMachine();
            enemyRoamState = new EnemyRoamState(this, _stateMachine);
            enemyChaseState = new EnemyChaseState(this, _stateMachine);
            enemyAttackState = new EnemyAttackDirState(this, _stateMachine);
            _stateMachine.SetStartState(enemyRoamState);
        }


        public override void UpdateLogic()
        {
            base.UpdateLogic();
        }

        public override void FixedUpdateLogic()
        {
            base.FixedUpdateLogic();
        }

        public override void RequestAttackToken()
        {
            base.RequestAttackToken();
        }

    }

}
