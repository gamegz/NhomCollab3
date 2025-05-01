using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateChase : EnemyChaseState
    {
        private float _chaseToAttackTimeCount;

        public EnemyStateChase(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            _chaseToAttackTimeCount = _enemy.chaseToAttackTransitTime;
            
        }

      
        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            if(_enemy.playerRef == null) {return;} 
            _enemy.UpdateLogicByPlayerDistance();

            _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);


            switch (_enemy.isTokenOwner)
            {
                case true:

                    

                    if (_enemy.canAttack)
                    {
                        _chaseToAttackTimeCount -= Time.deltaTime;
                        if(_chaseToAttackTimeCount <= 0)
                        {
                            _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
                            break;
                        }

                        if (_enemy.isTargetInAttackRange)
                        {
                            _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
                            break;
                        }
                    }
                    else
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                    }

                    break;
                case false:
                    _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
                    break;
            }

            
        }
        
        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

