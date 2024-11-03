using Enemy;
using Enemy.statemachine.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.variant
{
    public class EnemyThug : EnemyBase
    {

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
            currentSpeed = followSpeed;
        }

        public override void SetUpStateMachine()
        {
            base.SetUpStateMachine();
            enemyRoamState = new EnemyStateRoam(this, _stateMachine);
            enemyChaseState = new EnemyStateChase(this, _stateMachine);
            enemyAttackState = new EnemyStateAttackDash(this, _stateMachine);
            enemyRetreatState = new EnemyStateRetreat(this, _stateMachine);
            enemyFollowState = new EnemyStateFollow(this, _stateMachine);
            _stateMachine.SetStartState(enemyRoamState);       
            //currentState = EnemyState.Roam;
        }



        public override void UpdateLogic()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isTokenOwner = !isTokenOwner;
                //InnitDash(- GetPerpendicularVectorToTarget());
                //Debug.Log(enemyNavAgent.CalculatePath(target.transform.position, enemyNavAgent.path));
            }
            base.UpdateLogic();
        }

        public override void FixedUpdateLogic()
        {
            base.FixedUpdateLogic();
        }

    }

}
