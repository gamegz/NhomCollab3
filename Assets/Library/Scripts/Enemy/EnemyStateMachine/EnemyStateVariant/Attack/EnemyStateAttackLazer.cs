using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy.statemachine.States
{
    public class EnemyStateAttackLazer : EnemyAttackState
    {
        private float _attackInnitTimeCount;
        private float _attackDuration;
        private Vector3 _attackDir;

        private float lazerGuideTimeOffset = 0.7f;
        private float lazerGuideTimeCount;

        private float turnSpeed;

        public EnemyStateAttackLazer(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            _enemy.canTurn = true;
            _attackInnitTimeCount = _enemy.attackInnitTime;

            lazerGuideTimeCount = _attackInnitTimeCount - lazerGuideTimeOffset;
            lazerGuideTimeCount = Mathf.Clamp(lazerGuideTimeCount, 0, _attackInnitTimeCount);

            _attackDuration = _enemy.totalAttackDuration;
            Vector3 attackChargePos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                  _enemy.playerRef.transform.position - _enemy.transform.position,
                                                                  _enemy.distanceToPlayer * 1.6f, 1);
            _enemy.enemyNavAgent.SetDestination(attackChargePos);
            _attackDir = _enemy.GetDirectionToPlayer();
            _enemy.currentSpeed = _enemy.chaseSpeed;
            turnSpeed = _enemy.turnSpeed;
            _enemy.turnSpeed = 80;
            _enemy.isAttacking = true;
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
            _attackDuration -= Time.deltaTime;

            // Effecttttttttttttttttttttttttttttttttttttttttttttttttttttttt
            if (_attackInnitTimeCount > 0 && !_attackIndicatorPlayed)
            {
                _enemy.PlayAttackIndicatorSound();
                _enemy.PlayAttackIndicatorEffect();
                _attackIndicatorPlayed = true;
            }

            
            if (_attackInnitTimeCount <= 0)
            {         
                _enemy.enemyAnimators.SetTrigger("Attack");
                _enemy.ShootRayAttack(_attackDir);
            }

            if(lazerGuideTimeCount > 0)
            {
                lazerGuideTimeCount -= Time.deltaTime;
                _attackDir = _enemy.GetDirectionToPlayer();
            }
            else
            {
                _enemy.canTurn = false;
                _enemy.enemyNavAgent.SetDestination(_enemy.transform.position);
            }
          

            if (_attackDuration < 0) //Finish attack
            {
                _enemy.isAttacking = false;
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

        }
        
        public override void ExitState()
        {
            base.ExitState();
            _enemy.canTurn = true;
            _enemy.turnSpeed = turnSpeed;
        }
    }
}

