using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.statemachine.States
{
    public class EnemyStateRetreatRoam : EnemyRetreatState
    {
        private float _runDistance = 5;
        private float _runPosOffSet = 1;
        private float _retreatTime = 3;
        private float _retreatTimeCount;

        private NavMeshHit hit;
        private NavMeshPath path;

        //usually use for farrange enemy
        public EnemyStateRetreatRoam(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            _enemy.currentSpeed = _enemy.retreatSpeed;
            _retreatTimeCount = _retreatTime;
        }

      
        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            _enemy.UpdateLogicByPlayerDistance();
            Vector3 retreatPos = _enemy.GetNavLocationByDirection(_enemy.transform.position,
                                                                  _enemy.transform.position - _enemy.playerRef.transform.position,
                                                                  _runDistance, _runPosOffSet);
            _enemy.enemyNavAgent.SetDestination(retreatPos);


            _retreatTimeCount -= Time.deltaTime;
            if(_retreatTimeCount <= 0)
            {
                _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
            }            

            if(NavMesh.CalculatePath(_enemy.transform.position, _enemy.playerRef.transform.position, -1, path)){
                if(path.corners.Length >= 2)
                {
                    _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                }
            }

            


            switch (_enemy.isTokenOwner)
            {
                case true:
                    if (!_enemy.canAttack)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                        break;
                    }

                    if (_enemy.isTargetInAttackRange)
                    {   
                        if(!_enemy.canAttack) { return; }
                        _ownerStateMachine.SwitchState(_enemy.enemyAttackState);                       
                    }
                    else
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyChaseState);                       
                    }
                    break;
                   
                case false:
                    if(_enemy.distanceToPlayer < _enemy.maxRetreatDistance)
                    {                        
                        break;
                    }

                    //To far then follow otherwise roam
                    if (_enemy.distanceToPlayer > _enemy.followDistance)
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyFollowState);
                    }
                    else
                    {
                        _ownerStateMachine.SwitchState(_enemy.enemyRoamState);
                    }
                    break;

            }
        }
        
        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

