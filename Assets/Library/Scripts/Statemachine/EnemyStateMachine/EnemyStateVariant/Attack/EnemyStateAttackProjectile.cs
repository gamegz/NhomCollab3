using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateAttackProjectile : EnemyAttackState
    {
        private float _attackInnitTimeCount;
        private float _attackDuration;
        private Vector3 _attackDir;

        public EnemyStateAttackProjectile(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            _attackInnitTimeCount = _enemy.attackInnitTime;
            _attackDuration = _enemy.totalAttackDuration;
            Vector3 attackChargePos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                  _enemy.playerRef.transform.position - _enemy.transform.position,
                                                                  _enemy.distanceToPlayer * 1.6f, 1);
            _enemy.enemyNavAgent.SetDestination(attackChargePos);
            _attackDir = _enemy.GetDirectionToPlayer();
            _enemy.currentSpeed = _enemy.chaseSpeed;
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
                if(_attackInnitTimeCount <= 0)
                {
                    _enemy.ShootProjectile(_enemy.GetDirectionToPlayer());
                    _enemy.isAttacking = true; //Start attack
                }
                
            }


           

            //if (_enemy.GetDistanceToPLayer() < _enemy.retreatDistance)
            //{
            //    Vector3 retreatPos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
            //                                                     _enemy.transform.position - _enemy.playerRef.transform.position,
            //                                                     3.3f, 2);
            //    _enemy.enemyNavAgent.SetDestination(retreatPos);
            //}


            if (!_enemy.isAttacking) { return; }
            _attackDuration -= Time.deltaTime;
            if (_attackDuration < 0) //Finish attack
            {

                _enemy.OnDoneAttack();

                switch (_enemy.isTokenOwner)
                {
                    case true:
                        if (_enemy.GetDistanceToPLayer() < _enemy.retreatDistance)
                        {
                            _ownerStateMachine.SwitchState(_enemy.enemyRetreatState);
                            break;
                        }
                        
                        
                        _ownerStateMachine.SwitchState(_enemy.enemyFollowState);                       
                        break;
                    case false:

                        _ownerStateMachine.SwitchState(_enemy.enemyRetreatState);
                        break;
                }

                //if (_enemy.isTargetInAttackRange) //Attack again
                //{
                //    _attackInnitTimeCount = _enemy.attackInnitTime;
                //    _enemy.canTurn = true;
                //}
                //else //Chase
                //{
                //    _ownerStateMachine.SwitchState(_enemy.enemyChaseState);
                //}
                               
            }

        }
        
        public override void ExitState()
        {
            base.ExitState();
            _enemy.canTurn = true;
        }
    }
}

