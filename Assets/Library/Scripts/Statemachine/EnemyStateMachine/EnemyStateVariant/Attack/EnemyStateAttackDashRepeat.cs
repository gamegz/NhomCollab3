using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateAttackDashRepeat : EnemyAttackState
    {
        private float _attackInnitTimeCount;
        private float _attackDuration;
        private Vector3 _attackDir;

        private int _dashTimes = 3;
        private float _timeBetweenDash = 0.2f;
        private float _timeBetweenDashCount;

        private bool finishCurrentAttack;

        public EnemyStateAttackDashRepeat(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            _dashTimes = 3;
            _timeBetweenDashCount = 0;
            _attackInnitTimeCount = _enemy.attackInnitTime;
            _attackDuration = _enemy.DashDuration * _dashTimes + (_timeBetweenDash * (_dashTimes - 1));

            Vector3 attackChargePos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                  _enemy.playerRef.transform.position - _enemy.transform.position,
                                                                  _enemy.distanceToPlayer * 1.6f, 1);
            _enemy.enemyNavAgent.SetDestination(attackChargePos);
            _attackDir = _enemy.GetDirectionToPlayer();
            _enemy.canTurn = false;
            _enemy.canMove = false;
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

            _attackInnitTimeCount -= Time.deltaTime;
            if (_attackInnitTimeCount > 0) { return; }

            _enemy.isAttacking = true;
            _attackDuration -= Time.deltaTime;
            if (_attackDuration <= 0) //Finish attack
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

            }

            


            //switch (_dashTimes)
            //{
            //    case > 0:

            //        break;

            //    case <= 0:

            //        break;
            //}



            if (_dashTimes > 0)
            {
                _enemy.canTurn = true;

                _timeBetweenDashCount -= Time.deltaTime;
                if (_timeBetweenDashCount <= 0)
                {
                    Vector3 attackChargePos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                          _enemy.playerRef.transform.position - _enemy.transform.position,
                                          _enemy.distanceToPlayer * 1.6f, 1);
                    _enemy.enemyNavAgent.SetDestination(attackChargePos);
                    _attackDir = _enemy.GetDirectionToPlayer();
                    _enemy.LookAtTarget(_enemy.playerRef.transform.position);
                    _enemy.PresetDashAttack(_attackDir);

                    _dashTimes--;
                    _timeBetweenDashCount = _timeBetweenDash + _enemy.DashDuration;
                }

                
            }

            

            Debug.Log(_attackDuration);




            //Finish starting

            //_currentAttackDuration -= Time.deltaTime;
            //if(_currentAttackDuration <= 0)
            //{

            //    if (_dashTimes > 0)
            //    {                   
            //        _enemy.canTurn = true;

            //        _timeBetweenDashCount -= Time.deltaTime;
            //        if (_timeBetweenDashCount <= 0)
            //        {
            //            Vector3 attackChargePos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
            //                                  _enemy.playerRef.transform.position - _enemy.transform.position,
            //                                  _enemy.distanceToPlayer * 1.6f, 1);
            //            _enemy.enemyNavAgent.SetDestination(attackChargePos);
            //            _attackDir = _enemy.GetDirectionToPlayer();
            //            _enemy.LookAtTarget(_enemy.transform.position, _enemy.playerRef.transform.position);

            //            _timeBetweenDashCount = _timeBetweenDash;
            //            _currentAttackDuration = _enemy.DashDuration;
            //            _enemy.PresetDashAttack(_attackDir);
            //            _enemy.canTurn = true;
            //            _dashTimes--;

            //        }
            //    }

            //}

            

        }
        
        public override void ExitState()
        {
            base.ExitState();
            _enemy.canTurn = true;
            _enemy.isAttacking = false;
        }
    }
}

