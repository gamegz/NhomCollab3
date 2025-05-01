using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Enemy.statemachine.States;

namespace Enemy.variant
{
    public class EnemyHotEye : EnemyBase
    {
        private AudioSource _audioSource;
        public override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
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
            enemyAttackState = new EnemyStateAttackLazer(this, _stateMachine);
            enemyRetreatState = new EnemyStateRetreatRoam(this, _stateMachine);
            enemyFollowState = new EnemyStateFollow(this, _stateMachine);
            _stateMachine.SetStartState(enemyRoamState);
            //currentState = EnemyState.Roam;
        }

        public override void ShootRayAttack(Vector3 direction)
        {
            if (isStagger) return;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, layerData.hostileTargetLayer))
            {
                if (hit.transform.gameObject.CompareTag("Player"))
                {
                    hit.transform.gameObject.GetComponent<IDamageable>().TakeDamage(attackDamage);
                    _audioSource?.Play();
                }
                
            }
            
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