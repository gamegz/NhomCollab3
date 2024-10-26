using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.statemachine.States
{
    public class EnemyRoamState : BaseEnemyState
    {
        public EnemyRoamState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            
        }

        public override void EnterState()
        {
            
        }

        public override void ExitState()
        {
            
        }

        public override void FixedUpdateS()
        {
            
        }

        public override void UpdateState()
        {
            
        }

        //public Vector3 RandomNavmeshLocation(float radius)
        //{
        //    Vector3 randomDirection = Random.insideUnitSphere * radius;
        //    randomDirection += transform.position;
        //    NavMeshHit hit;
        //    Vector3 finalPosition = Vector3.zero;
        //    if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        //    {
        //        finalPosition = hit.position;
        //    }
        //    return finalPosition;
        //}
    }
}

