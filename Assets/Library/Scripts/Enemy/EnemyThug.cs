using Enemy;
using Enemy.statemachine.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.variant
{
    public class EnemyThug : EnemyBase
    {
        protected EnemyRoamState _enemyRoamState;

        public override void Awake()
        {
            base.Awake();
        }

        public override void SetUpStateMachine()
        {
            base.SetUpStateMachine();
            _enemyRoamState = new EnemyRoamState(this, _stateMachine);
            _stateMachine.SetStartState(_enemyRoamState);
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
