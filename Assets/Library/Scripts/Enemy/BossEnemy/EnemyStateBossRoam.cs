using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    //This class runs when the boss resting between movesets
    //Transition to attack default when countdown is over
    public class EnemyStateBossRoam : EnemyRoamState
    {
        [SerializeField] private float roamSpeed = 40;
        private float roamDirChangeDelayTime = 1;

        private BossEnemyBase bossEnemy;

        private float roamTimeCount;
        private float roamDirChangeDelayTimeCount;

        public EnemyStateBossRoam(BossEnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
            roamTimeCount = bossEnemy.roamTime;            
            roamDirChangeDelayTimeCount = 0;
            base.EnterState();
            bossEnemy.currentSpeed = roamSpeed;
        }

        public override void UpdateState()
        {            
            base.UpdateState();
            if (roamTimeCount > 0)
            {
                roamTimeCount -= Time.deltaTime;

                roamDirChangeDelayTimeCount -= Time.deltaTime;
                if (roamDirChangeDelayTimeCount <= 0)
                {
                    float roamRange = Random.Range(3, 8);
                    Vector3 roamLocation = bossEnemy.GetRandomNavmeshLocationAroundSelf(roamRange);
                    bossEnemy.enemyNavAgent.SetDestination(roamLocation);
                    roamDirChangeDelayTime = Random.Range(1.2f, 2.5f);
                    roamDirChangeDelayTimeCount = roamDirChangeDelayTime;
                }               
                return;
            }

            
            _ownerStateMachine.SwitchState(bossEnemy.bossAttackDefault);
        }

        public override void FixedUpdateS()
        {
            base.FixedUpdateS();
        }
        

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}
