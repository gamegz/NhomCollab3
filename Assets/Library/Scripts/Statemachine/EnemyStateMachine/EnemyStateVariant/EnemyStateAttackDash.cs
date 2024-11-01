using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateAttackDash : EnemyAttackState
    {
        private float _attackInnitTimeCount;
        private float _attackDuration;
        private bool _isAttacking;
        private bool _canAttack;

        public EnemyStateAttackDash(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            _attackInnitTimeCount = _enemy.attackInnitTime;
            _attackDuration = _enemy.DashDuration;
            Vector3 attackChargePos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                  _enemy.playerRef.transform.position - _enemy.transform.position,
                                                                  _enemy.distanceToPlayer * 1.6f, 1);

            _canAttack = true;
            _enemy.enemyNavAgent.SetDestination(attackChargePos);
        }

      
        public override void FixedUpdateS()
        {
            base.FixedUpdateS();
        }

        public override void UpdateState()
        {
            base.FixedUpdateS();
            _enemy.UpdateLogicByPlayerDistance();

            //_attackCoolDownCount -= Time.deltaTime;
            

            if (_attackInnitTimeCount > 0)
            {
                _attackInnitTimeCount -= Time.deltaTime;
                _enemy.canMove = false;
                if(_attackInnitTimeCount <= 0)
                {
                    _enemy.PresetDashAttack(_enemy.GetDirectionToPlayer());
                    _isAttacking = true;
                }
                
            }

            if (_isAttacking)
            {
                _attackDuration -= Time.deltaTime;
                if (_attackDuration < 0) //Finish attack
                {
                    _attackDuration = _enemy.DashDuration;
                    if (_enemy.isTargetInAttackRange) //Attack again
                    {
                        _attackInnitTimeCount = _enemy.attackInnitTime;
                        _isAttacking = false;
                    }
                    else //Chase
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
                    }
                    
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

