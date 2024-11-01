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
        #region DATA CONFIG

        public GameObject playerRef;
        public ActorLayerData layerData;
        public Rigidbody rb;
        public CapsuleCollider colliderCapsule;
        [Space]
        
        [SerializeField] private int maxHealth;
        [HideInInspector] public int currentHealth;

        //Combat
        [Header("COMBAT")]
        public Collider attackCollider;
        public int attackDamage;
        public float attackInnitTime;
        public float attackRecoverTime;
        public float attackCooldown;
        public float attackRange;
        public float chaseRange;
        public float retreatDistance;

        [Tooltip("Amount of damage recive before stagger")]
        public float staggerThreshold;
        private float _staggerThresholdCount;
        public float staggerTime;
        private float _currentStaggerTimeLeft;
        [HideInInspector] public bool isStagger = false;
        [HideInInspector] public bool isTargetInAttackRange;
        [HideInInspector] public bool isTargetInChaseRange;
        [HideInInspector] public bool isTokenOwner;



        //Navigation
        [Header("NAVIGATION")]
        public NavMeshAgent enemyNavAgent;
        public float followSpeed;
        public float chaseSpeed;
        public float roamSpeed;
        public float turnSpeed;
        [SerializeField] private float _dashDistance;
        [SerializeField] private float _dashDuration;
        public float DashDuration { get { return _dashDuration; }}
        [HideInInspector] public float currentSpeed;
        [HideInInspector] public bool isDashing;
        [HideInInspector] public bool canMove;
        [HideInInspector] public bool canTurn;
        [HideInInspector] public bool isActive;
        [HideInInspector] public float distanceToPlayer;
        public float roamRadius;
        public float roamCountDown;
        public float roamDuration;
        

        
        [Header("STATEMACHINE")]
        protected EnemyStateMachine _stateMachine;
        public EnemyRoamState enemyRoamState;
        public EnemyAttackState enemyAttackState;
        public EnemyChaseState enemyChaseState;
        public EnemyFollowState enemyFollowState;
        public EnemyRetreatState enemyRetreatState;
        [HideInInspector] public EnemyState currentState;

        [Header("DeathConfig")]
        [SerializeField] private int _dropValue;
        public DeathMethod deathMethod;

        public enum EnemyState { 
            Roam,
            Chase,
            Follow,
            Retreat,
            Attack
        }

        public enum DeathMethod
        {
            KineticCut,
            KineticSmash,
            Burn,
            Freeze,
        }

        #endregion


        public virtual void Awake()
        {                     
            SetUpStateMachine();
            
        }

        public virtual void SetUpStateMachine()
        {
            _stateMachine = new EnemyStateMachine();
            enemyRoamState = new EnemyRoamState(this, _stateMachine);
            enemyChaseState = new EnemyChaseState(this, _stateMachine);
            enemyAttackState = new EnemyAttackState(this, _stateMachine);
            enemyFollowState = new EnemyFollowState(this, _stateMachine);
            enemyRetreatState = new EnemyRetreatState(this, _stateMachine);

        }

        public virtual void Start()
        {
            enemyNavAgent.updateRotation = false;
            canMove = true;
            canTurn = true;
            currentSpeed = roamSpeed;
            currentHealth = maxHealth;

            attackCollider.enabled = true;
            _currentStaggerTimeLeft = staggerTime;            
            _staggerThresholdCount = staggerThreshold;

            isTokenOwner = true;
        }

        public virtual void UpdateLogic()
        {
            if(isDashing) { return; }
            enemyNavAgent.speed = (canMove) ? currentSpeed / 10 : 0;
            LookAtTarget(transform, playerRef.transform, turnSpeed);
            UpdateStaggerLogic();
            _stateMachine.UpdateState();
        }

        public virtual void FixedUpdateLogic()
        {
            if (isDashing) { return; }
            _stateMachine.FixedUpdateState();
        }

        private void OnCollisionEnter(Collision collision)
        {
            //myRigidbody.velocity = Vector3.ProjectOnPlane(myRigidbody.velocity, collision.contacts[0].normal);
        }

        public void UpdateLogicByPlayerDistance()
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);
            isTargetInAttackRange = (distanceToPlayer <= attackRange) ? true : false;
            isTargetInChaseRange = (distanceToPlayer <= chaseRange) ? true : false;
        }

        private void UpdateStaggerLogic()
        {
            if (!isStagger) { return; }

            if (_currentStaggerTimeLeft > 0)
            {

                _currentStaggerTimeLeft -= Time.deltaTime;
            }
            else
            {
                _currentStaggerTimeLeft = staggerTime;
                _staggerThresholdCount = staggerThreshold;
                isStagger = true;
            }
        }

        #region LOGIC IMPLEMENT

        #region MOVEMENT DASH
        public void InnitDash(Vector3 dashDirection)
        {
            StartCoroutine(EnemyDash(dashDirection));
        }
        private IEnumerator EnemyDash(Vector3 dashDirection)
        {
            if (isDashing) { yield break; }

            dashDirection = dashDirection.normalized;
            float dashDistance = _dashDistance;
            Vector3 dashPoint = transform.position + dashDirection * dashDistance;
            NavMeshHit hit;

            Vector2 enemyPos = new Vector2(transform.position.x, transform.position.z);

            if (enemyNavAgent.CalculatePath(dashPoint, enemyNavAgent.path)) //Inside navmesh
            {
                //Path blocked, Get distance from block point
                if (NavMesh.Raycast(transform.position, dashPoint, out hit, NavMesh.AllAreas))
                {
                    //dashPoint = (transform.position + dashDirection) * (_dashDistance - (Vector2.Distance(hit.position, dashPoint) + colliderCapsule.radius));
                    dashDistance = Vector2.Distance(enemyPos, new Vector2(hit.position.x, hit.position.z)) - colliderCapsule.radius * 2;
                }
            }
            else //Outside navmesh
            {
                NavMeshHit hitData;
                switch (enemyNavAgent.Raycast(dashPoint, out hitData))
                {
                    case true: //Path blocked, Get distance from block point
                        dashPoint = hitData.position;
                        dashDistance = Vector2.Distance(enemyPos, new Vector2(hitData.position.x, hitData.position.z)) - colliderCapsule.radius * 2;
                        break;

                    case false: //Is not blocked, Sample to get the rear position
                        Debug.LogWarning("Cuh what the hell, ho could this happen");
                        if (NavMesh.SamplePosition(dashPoint, out hitData, 0.1f, 1))
                        {
                            dashPoint = hitData.position;
                            dashDistance = Vector2.Distance(enemyPos, new Vector2(hitData.position.x, hitData.position.z)) - colliderCapsule.radius * 2;
                        }
                        break;
                }
            }

            //Start
            canTurn = false;
            canMove = false;
            //enemyNavAgent.updateRotation = false;
            LookAtTarget(transform.position, dashPoint);
            //enemyNavAgent.updatePosition = false;

            isDashing = true;

            //Dashing
            float dashSpeed = dashDistance / _dashDuration;
            float dashDurationCount = _dashDuration;

            while (dashDurationCount > 0)
            {
                enemyNavAgent.velocity = dashDirection * dashSpeed;
                dashDurationCount -= Time.deltaTime;
                yield return null;
            }


            enemyNavAgent.velocity = Vector3.zero;
            //enemyNavAgent.updateRotation = true;
            //enemyNavAgent.updatePosition = true;
            canTurn = true;
            canMove = true;
            isDashing = false;
        }

        #endregion
        #region MOVEMENT TURNING

        private void LookAtTarget(Transform center, Transform target, float speed)
        {
            if (!canTurn)
            {
                return;
            }

            Vector3 dirToTarget = target.position - center.position;
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = Quaternion.Lerp(center.rotation, rotation, Time.deltaTime * speed);
        }

        private void LookAtTarget(Transform center, Transform target)
        {
            if (!canTurn) { return; }
            Vector3 dirToTarget = target.position - center.position;
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = rotation;
        }

        private void LookAtTarget(Vector3 center, Vector3 target)
        {
            if (!canTurn) { return; }
            Vector3 dirToTarget = target - center;
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = rotation;
        }
        #endregion

        public virtual void PresetDashAttack(Vector3 DashDirection, float attackTimeOffSet = 0)
        {
            InnitDash(DashDirection);
            InnitAttackCollider(_dashDuration);
        }

        public IEnumerator InnitAttackCollider(float duration)
        {
            attackCollider.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            attackCollider.gameObject.SetActive(false);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            _staggerThresholdCount -= damage;

            if (_staggerThresholdCount <= 0 && !isStagger)
            {
                isStagger = true;
            }

            if (currentHealth > 0) { return; }
            OnDeath();
        }

        public virtual void OnDeath()
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

        #endregion



        #region HELPER

        public Vector3 GetRandomNavmeshLocation()
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

        public Vector3 GetNavLocationByDirection(Vector3 startingPoint, Vector3 direction, float checkDistance, float checkRadiusAroundHitLocation)
        {
            Vector3 location = startingPoint + direction.normalized * checkDistance;
            NavMeshHit hitData;
            if (NavMesh.SamplePosition(location, out hitData, checkRadiusAroundHitLocation, 1))
            {
                location = hitData.position;
            }

            return location;
        }

        //Get direction that is perpendicular to the direction from target to self
        public Vector3 GetPerpendicularVectorToTarget()
        {

            Vector3 dirTargetToSelf = playerRef.transform.position - transform.position;
            Vector2 perdendicularVector = Vector2.Perpendicular(new Vector2(dirTargetToSelf.x, dirTargetToSelf.z));

            return new Vector3(perdendicularVector.x, transform.position.y, perdendicularVector.y);

        }

        public bool GetDestinationCompleteStatus()
        {
            if (enemyNavAgent.pathPending) { return false; } //Have path to run
            if (enemyNavAgent.remainingDistance > enemyNavAgent.stoppingDistance) { return false; } //Haven not reach destination
            if (!enemyNavAgent.hasPath) //does not have a path
            {
                return true;
            }
            return false;
        }

        public Vector3 GetDirectionToPlayer()
        {
            return playerRef.transform.position - transform.position;
        }

        public Vector3 GetDirectionIgnoreY(Vector3 from, Vector3 to)
        {
            return new Vector3(to.x, 2, to.z) - new Vector3(from.x, 2, from.y);
        }

        #endregion


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

