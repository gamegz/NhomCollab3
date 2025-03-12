using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyStateBossChase : EnemyAttackState
    {
        private BossEnemyBase bossEnemy;

        public EnemyStateBossChase(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {          
            bossEnemy = enemy;
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void FixedUpdateS()
        {
            base.FixedUpdateS();
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}
