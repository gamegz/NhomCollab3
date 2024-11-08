using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateChaseHassleAttack : EnemyChaseState
    {
        

        public EnemyStateChaseHassleAttack(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            
        }

      
        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            _enemy.UpdateLogicByPlayerDistance();

            _enemy.enemyNavAgent.SetDestination(_enemy.playerRef.transform.position);


            switch (_enemy.isTokenOwner)
            {
                case true:
                    if (_enemy.isTargetInAttackRange && _enemy.canAttack)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyAttackState);
                    }
                    else if (!_enemy.canAttack)
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

