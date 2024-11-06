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

        public EnemyHostileMethod hostileMethod; //Put this as gunner for testing enemy without tokensystem
        public GameObject playerRef;
        public ActorLayerData layerData;
        public Rigidbody rb;
        public CapsuleCollider colliderCapsule;
        [Space]
        
        

        //Combat
        [Header("COMBAT")]
        [SerializeField] private int maxHealth;
        [HideInInspector] public int currentHealth;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private GameObject _shootProjectile;
        public Collider attackCollider;
        public int attackDamage;
        [Tooltip("Usually overwritten by attack script")]
        public float totalAttackDuration;
        public float attackInnitTime;
        public float attackCooldown;
        [HideInInspector] public bool isAttacking;
        [HideInInspector] public bool canAttack;
        [HideInInspector] public float attackCoolDownCount;
        public float attackRange;
        public float chaseToAttackTransitTime;
        [Tooltip("Distance to start retreat (The closest player can reach)")]
        public float retreatDistance;
        [Tooltip("Max Distance to transtion from retreat (Must be greater than retreatDistance)")]
        public float maxRetreatDistance;
        [Tooltip("Distance to return if player too far away")]
        public float followDistance;

        [Tooltip("Amount of damage recive before stagger")]
        public float staggerThreshold;
        private float _staggerThresholdCount;
        public float staggerTime;
        private float _currentStaggerTimeLeft;
        [HideInInspector] public bool isStagger = false;
        [HideInInspector] public bool isTargetInAttackRange;
        [HideInInspector] public bool isTokenOwner;



        //Navigation
        [Header("NAVIGATION")]
        public NavMeshAgent enemyNavAgent;
        public float followSpeed;
        public float retreatSpeed;
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
        [Tooltip("Time until change roam position")]
        public float roamCountDown;
        [Tooltip("Time to roam until check for transition")]
        public float roamDuration;
        

        
        [Header("STATEMACHINE")]
        protected EnemyStateMachine _stateMachine;
        [HideInInspector] public EnemyRoamState enemyRoamState;
        [HideInInspector] public EnemyAttackState enemyAttackState;
        [HideInInspector] public EnemyChaseState enemyChaseState;
        [HideInInspector] public EnemyFollowState enemyFollowState;
        [HideInInspector] public EnemyRetreatState enemyRetreatState;
        [HideInInspector] public EnemyState currentState;

        [Header("DeathConfig")]
        [SerializeField] private int _dropValue;
        public DeathMethod deathMethod;

        public enum EnemyHostileMethod
        {
            Melee,
            Gunner, //Unaffected by token
            MeleeGunner
        }

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
            canAttack = true;
            currentSpeed = roamSpeed;
            currentHealth = maxHealth;

            attackCollider.enabled = false;
            attackCollider.gameObject.SetActive(false);
            _currentStaggerTimeLeft = staggerTime;            
            _staggerThresholdCount = staggerThreshold;
            attackCoolDownCount = attackCooldown;

            if (hostileMethod == EnemyHostileMethod.Gunner) { isTokenOwner = true; }
            attackCollider.gameObject.GetComponent<EnemyAttackCollider>()._damage = attackDamage;
        }

        public virtual void UpdateLogic()
        {

            //Debug.Log(currentState);
            UpdateAttackCoolDown();
            _stateMachine.UpdateState();
            if (isDashing) { return; }
            enemyNavAgent.speed = (canMove) ? currentSpeed / 10 : 0;
            LookAtTarget(transform, playerRef.transform, turnSpeed);
            
            

            
        }

        public virtual void FixedUpdateLogic()
        {
            if (isDashing) { return; }
            _stateMachine.FixedUpdateState();
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    //myRigidbody.velocity = Vector3.ProjectOnPlane(myRigidbody.velocity, collision.contacts[0].normal);
        //}

        public void UpdateLogicByPlayerDistance()
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);
            isTargetInAttackRange = (distanceToPlayer <= attackRange) ? true : false;
        }

        //private void UpdateStaggerLogic()
        //{
        //    if (!isStagger) { return; }

        //    if (_currentStaggerTimeLeft > 0)
        //    {

        //        _currentStaggerTimeLeft -= Time.deltaTime;
        //    }
        //    else
        //    {
        //        _currentStaggerTimeLeft = staggerTime;
        //        _staggerThresholdCount = staggerThreshold;
        //        isStagger = true;
        //    }
        //}

        private void UpdateAttackCoolDown() {
        
            //note: call OnDonwAttack when finish attack
            if (!canAttack) {
                attackCoolDownCount -= Time.deltaTime;
                canAttack = (attackCoolDownCount <= 0) ? true : false;
            }
        }  //Allow attack again after a delay

        public void OnDoneAttack() {   
            attackCoolDownCount = attackCooldown;
            canAttack = false;
            isAttacking = false;
        } //Call when finish attack

        #region LOGIC IMPLEMENT

        #region MOVEMENT DASH
        public void InnitDash(Vector3 dashDirection)
        {
            StartCoroutine(EnemyDash(dashDirection, _dashDistance, _dashDuration));
        }
        private IEnumerator EnemyDash(Vector3 dashDirection, float DashDistance, float DashTime)
        {
            if (isDashing) { yield break; }

            canTurn = false;
            canMove = false;
            //enemyNavAgent.updateRotation = false;           
            enemyNavAgent.ResetPath();
            //enemyNavAgent.updatePosition = false;
            isDashing = true;

            dashDirection = dashDirection.normalized;
            float dashDistance = DashDistance;
            Vector3 dashPoint = transform.position + dashDirection * dashDistance;
            LookAtTarget(transform.position, dashPoint);
            NavMeshHit hit;

            Vector2 enemyPos = new Vector2(transform.position.x, transform.position.z);

            NavMeshPath navpath = new NavMeshPath();

            //Note : recaculate distance and shit use when assign layers
            //if (enemyNavAgent.CalculatePath(dashPoint, enemyNavAgent.path)) //Inside navmesh
            //{
            //    //Path blocked, Get distance from block point
            //    if (NavMesh.Raycast(transform.position, dashPoint, out hit, NavMesh.AllAreas))
            //    {
            //        //dashPoint = (transform.position + dashDirection) * (_dashDistance - (Vector2.Distance(hit.position, dashPoint) + colliderCapsule.radius));
            //        dashDistance = Vector2.Distance(enemyPos, new Vector2(hit.position.x, hit.position.z)) - colliderCapsule.radius * 2;
            //    }
            //}
            //else //Outside navmesh
            //{
            //    NavMeshHit hitData;
            //    switch (enemyNavAgent.Raycast(dashPoint, out hitData))
            //    {
            //        case true: //Path blocked, Get distance from block point
            //            dashPoint = hitData.position;
            //            dashDistance = Vector2.Distance(enemyPos, new Vector2(hitData.position.x, hitData.position.z)) - colliderCapsule.radius * 2;
            //            break;

            //        case false: //Is not blocked, Sample to get the rear position
            //            Debug.LogWarning("Cuh what the hell, how could this happen");
            //            if (NavMesh.SamplePosition(dashPoint, out hitData, 0.1f, 1))
            //            {
            //                dashPoint = hitData.position;
            //                dashDistance = Vector2.Distance(enemyPos, new Vector2(hitData.position.x, hitData.position.z)) - colliderCapsule.radius * 2;
            //            }
            //            break;
            //    }
            //}

            if (NavMesh.SamplePosition(dashPoint, out hit, 0.1f, 1))
            {
                dashPoint = hit.position;
                dashDistance = Vector2.Distance(enemyPos, new Vector2(hit.position.x, hit.position.z)) - colliderCapsule.radius * 2;
            }
            

            //Recaculate dash duration when distance is changed
            //DashTime *= (DashDistance / dashDistance);

            //Dashing
            float dashSpeed = dashDistance / DashTime;
            float dashDurationCount = DashTime;

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

        public void LookAtTarget(Transform center, Transform target, float speed)
        {
            if (!canTurn)
            {
                return;
            }

            Vector3 dirToTarget = target.position - center.position;
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = Quaternion.Lerp(center.rotation, rotation, Time.deltaTime * speed);
        }

        public void LookAtTarget(Transform center, Transform target)
        {
            if (!canTurn) { return; }
            Vector3 dirToTarget = target.position - center.position;
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = rotation;
        }

        public void LookAtTarget(Vector3 center, Vector3 target)
        {
            if (!canTurn) { return; }
            Vector3 dirToTarget = target - center;
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = rotation;
        }
        #endregion

        #region ATTACK
        public virtual void PresetDashAttack(Vector3 DashDirection, float attackTimeOffSet = 0)
        {
            InnitDash(DashDirection);
            InnitAttackCollider(_dashDuration);
        }

        public void InnitAttackCollider(float duration)
        {
            StartCoroutine(DisableEnableAttackCollider(duration));
        }
        private IEnumerator DisableEnableAttackCollider(float duration)
        {
            attackCollider.gameObject.SetActive(true);
            attackCollider.enabled = true;
            yield return new WaitForSeconds(duration);
            attackCollider.enabled = false;
            attackCollider.gameObject.SetActive(false);
        }

        public void ShootProjectile(Vector3 direction)
        {
            GameObject projectile = Instantiate(_shootProjectile, _shootPoint.position, Quaternion.identity);
            if (projectile.TryGetComponent<EnemyProjectile>(out EnemyProjectile enemyProjectile))
            {
                enemyProjectile.SetUp(direction, this.gameObject);
            }
            
        }

        #endregion



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
        public Vector3 GetPerpendicularDirectionToTarget(bool toRight = true)
        {

            Vector3 dirTargetToSelf = playerRef.transform.position - transform.position;
            Vector2 perdendicularVector = Vector2.Perpendicular(new Vector2(dirTargetToSelf.x, dirTargetToSelf.z)).normalized;

            Vector3 finalDir = new Vector3(perdendicularVector.x, transform.position.y, perdendicularVector.y);
            finalDir = (toRight) ? finalDir : -finalDir;

            return finalDir;
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
            return (playerRef.transform.position - transform.position).normalized;
        }

        public Vector3 GetDirectionIgnoreY(Vector3 from, Vector3 to)
        {
            return new Vector3(to.x, 2, to.z) - new Vector3(from.x, 2, from.y);
        }

        public float GetDistanceToPLayer()
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);
            return distanceToPlayer;
        } //Get straight distance to player
        public float GetNavMeshTrueDistanceToPlayer()
        {
            var corners = enemyNavAgent.path.corners;
            float distance = 0.5f;

            for (int i = 1; i < corners.Length; i++)
            {
                distance += Vector3.Distance(corners[i - 1], corners[i]);
            }

            return distance;
        } //Distance to player concerning the obstacles

        #endregion


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            //Gizmos.DrawLine(transform.position, GetPerpendicularVectorToTarget());
        }
    }

}

