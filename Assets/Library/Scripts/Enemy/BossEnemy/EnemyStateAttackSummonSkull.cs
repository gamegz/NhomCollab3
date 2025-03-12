using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyStateAttackSummonSkull : EnemyAttackState
    {
        private GameObject summonObj;
        private BossEnemyBase bossEnemy;

        public EnemyStateAttackSummonSkull(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine, GameObject summonObject) : base(enemy, enemyStateMachine)
        {
            summonObj = summonObject;
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

