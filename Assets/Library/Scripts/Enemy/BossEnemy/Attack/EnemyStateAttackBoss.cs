using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    //This class reference to all attacks and choose which attacks to continue with first
    //Other attack state transition to this if the moveset is finished to reevaluate
    public class EnemyStateAttackBoss : EnemyAttackState
    {
        [SerializeField] private float summonAttackRange = 17;
        [SerializeField] private float meleeAttackRange = 8;
        [SerializeField] private float restTimePerMove = 1.1f;


        private BossEnemyBase bossEnemy;
        
        

        public EnemyStateAttackBoss(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            bossEnemy = enemy;
        }

        public override void SetUpState(EnemyBase enemy, EnemyStateMachine enemyStateMachine)
        {
            bossEnemy = (BossEnemyBase)enemy;
            base.SetUpState(enemy, enemyStateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();

            float healthDifference = bossEnemy.currentHealth / bossEnemy.MaxHealth;
            switch (healthDifference)
            {
                case <= 0.2f:
                    bossEnemy.attackMoveNum = Random.Range(2, 6);
                    break;
                default:
                    bossEnemy.attackMoveNum = Random.Range(3, 5);
                    break;
            }
            bossEnemy.roamTime = restTimePerMove * bossEnemy.attackMoveNum;
        }


        public override void FixedUpdateS()
        {
            base.FixedUpdateS();
        }

        public override void UpdateState()
        {          
            base.FixedUpdateS();
            float playerDistance = bossEnemy.GetDistanceToPLayerIgnoreY();
            if (playerDistance > summonAttackRange)
            {
                _ownerStateMachine.SwitchState(bossEnemy.enemyAttackRanged1);
                
            }
            else if(playerDistance > meleeAttackRange) //Do summon attacks
            {
                int randNum = Random.Range(1, 3);
                switch (randNum)
                {
                    case 1:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon1);
                        break;
                    case 2:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackSummon2);
                        break;
                }
            }
            else if (playerDistance <= meleeAttackRange) //Do melee attacks
            {
                int randNum = Random.Range(1, 3);
                switch (randNum)
                {
                    case 1:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee1);
                        break;
                    case 2:
                        _ownerStateMachine.SwitchState(bossEnemy.enemyAttackMelee2);
                        break;
                }
            }

        }

        public override void ExitState()
        {
            base.ExitState();
            _enemy.canTurn = true;
        }
    }
}
