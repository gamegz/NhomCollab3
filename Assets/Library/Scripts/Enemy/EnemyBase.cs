using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.statemachine;
using Enemy.statemachine.States;
using UnityEngine.AI;


namespace Enemy
{
    //Need EnemyManager to start running
    //Hold stats
    //Manage statemachine and states innitialization
    /*
    To create new enemy:
    Inherit from this class
    Add behavior by contructing statemachine states into child class
    Changing spawn and other option
    */
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        public GameObject playerRef;
        public ActorLayerData layerData;

        [SerializeField] private int maxHealth;
        [HideInInspector] public int currentHealth;

        //Combat
        public int attackDamage;
        public int attackRecoverTime;
        public float attackRange;
        public float chaseRange; //-
        [HideInInspector] public bool isTargetInAttackRange;
        [HideInInspector] public bool isTargetInChaseRange;
        [HideInInspector] public bool isTokenOwner;

        

        //Navigation
        public NavMeshAgent enemyNavAgent;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _chaseSpeed;
        [HideInInspector] public float currentSpeed;
        [HideInInspector] public bool canMove;

        public float roamRadius;
        public float roamDelay;

        [HideInInspector] public float distanceToPlayer; 
     
        //Drops upon death
        [SerializeField] private int _dropValue;

        //Statemachine
        protected EnemyStateMachine _stateMachine;  
        
        public enum DeathMethod
        {
            KineticCut,
            KineticSmash,
            Burn,
            Freeze,
        }
        public DeathMethod deathMethod => DeathMethod.KineticCut;
        


        public virtual void Awake()
        {
            currentHealth = maxHealth;
            SetUpStateMachine();
        }

        public virtual void SetUpStateMachine()
        {
            _stateMachine = new EnemyStateMachine();
        }

        private void Start()
        {
            enemyNavAgent.speed = _walkSpeed/10;
        }

        public virtual void UpdateLogic()
        {
            _stateMachine.UpdateState();
        }

        public virtual void FixedUpdateLogic()
        {
            _stateMachine.FixedUpdateState();
        }

        public virtual void RequestAttackToken()
        {
            
        }
        public virtual void ReturnAttackToken()
        {

        }

        public void Damage(int damage)
        {
            currentHealth -= damage;
            //Play damaged animation

            if(currentHealth > 0) { return; }

            OnDeath();
        }

        public void OnDeath()
        {
            //Disable stuff
            //Play particle or death animation, maybe?
            switch (deathMethod)
            {
                case DeathMethod.KineticCut:
                    break;
                case DeathMethod.KineticSmash:
                    break;
                case DeathMethod.Burn:
                    break;
                case DeathMethod.Freeze:
                    break;
            }
            Destroy(gameObject);

        }

        public virtual Vector3 GetRandomNavmeshLocation()
        {
            //Get a random point in a circle around target
            Vector3 randomDirection = transform.position + Random.insideUnitSphere * roamRadius;
            NavMeshHit hitData;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hitData, roamRadius, 1))
            {
                finalPosition = hitData.position;
            }

            //Debug.Log(finalPosition);
            return finalPosition;
        }

        public void UpdateLogicByPlayerDistance()
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);
            isTargetInAttackRange = (distanceToPlayer <= attackRange) ? true : false;
            isTargetInChaseRange = (distanceToPlayer <= chaseRange) ? true : false;
        }

        public bool GetDestinationCompleteStatus()
        {
            if (enemyNavAgent.pathPending) { return false; }
            if (enemyNavAgent.remainingDistance > enemyNavAgent.stoppingDistance) { return false; }
            if (!enemyNavAgent.hasPath || enemyNavAgent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
            return false;
        }

        //Get direction that is perpendicular to the direction from target to self
        public Vector3 GetPerpendicularVectorToTarget()
        {
            Vector3 dirTargetToSelf = playerRef.transform.position - transform.position;
            float x = 10; //Perpendicular => x*dir.x + y*dir.y + z*dir.z = 0
            float z = (dirTargetToSelf.x * 10) / - dirTargetToSelf.z;

            return new Vector3(x, 0, z).normalized * 10;

            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
            //Gizmos.DrawLine(transform.position, GetPerpendicularVectorToTarget());
        }
    }

}

