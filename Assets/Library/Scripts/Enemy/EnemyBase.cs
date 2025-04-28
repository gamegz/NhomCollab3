using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.statemachine;
using Enemy.statemachine.States;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Serialization;


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
        public static event Action<bool> OnEnemyDamaged;
        public event Action<EnemyBase> OnEnemyDeaths;
        public delegate void OnCallEnemyDeath(EnemyBase enemy);
        public static event OnCallEnemyDeath OnEnemyDeathsEvent;
        #region DATA CONFIG


        public GameObject playerRef;
        public StatsUpgrade playerStatsRef;
        public ActorLayerData layerData;
        public CapsuleCollider colliderCapsule;
        public GameObject expPrefab;
        [Space]



        //Combat
        [Header("COMBAT")]
        [SerializeField] protected int maxHealth; public int MaxHealth => maxHealth;
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


        [Space]
        [Header("Stagger")]
        [Tooltip("Amount of damage recive before stagger")]
        public float staggerThreshold;
        public float staggerTime;
        private float staggerTimeCounter;
        private float staggerThresholdCounter;
        public float knockbackForce = 0.0f;
        [HideInInspector] public bool isStagger = false;
        [HideInInspector] public bool isTargetInAttackRange;
        [HideInInspector] public bool isTokenOwner;
        [SerializeField] private bool isTokenUser = true;

        //Stun
        [Header("STUN")]
        public bool canBeStunned;
        public int stunPoint;// Percentage
        public float stunDuration;
        [HideInInspector] public bool isStunned = false;
        private bool hasBeenStunned = false;


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
        public float DashDuration { get { return _dashDuration; } }
        public float DashDistance { get { return _dashDistance; } }

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

        [SerializeField] private Animator enemyAnimator;

        public Animator enemyAnimators => enemyAnimator;


        public enum EnemyState
        {
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

        #region DUC ANH'S VARIABLE
        [Header("Values")]
        [SerializeField] private float timeLimitUI; // To calculate when enemy's health will fade away in the scene
        [SerializeField] private bool isBoss = false;
        [SerializeField] private float damageSmoothTime = 0.5f;
        private float damageTakenVelocity = 0f;

        private bool isChargedATK = false;



        [Header("References")]
        [SerializeField] private Image healthBackgroundUI = null; // To provide background for the enemy's health
        [SerializeField] private Image enemyHealthUI = null; // To display enemy's health
        [SerializeField] private Image enemyDamageReceivedUI = null;


        [Header("Coroutines")]
        private Coroutine uiAppearCoroutine = null;
        #endregion 
        
        
        //Effect
        [Header("Effects")]
        [SerializeField] private ParticleSystem bloodSpatterEffect;
        [SerializeField] private ParticleSystem slashSpatterEffect;
        [SerializeField] private ParticleSystem attackIndicatorEffect;


        public virtual void Awake()
        {
            SetUpStateMachine();

        }

        protected virtual void OnEnable()
        {
            WeaponManager.OnPerformChargedATK += HitByChargedATK;
            GameManager.OnPlayerDeathEvent += OnDeath;
        }

        protected virtual void OnDisable()
        {
            WeaponManager.OnPerformChargedATK -= HitByChargedATK;
            GameManager.OnPlayerDeathEvent -= OnDeath;
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
            isStagger = false;
            canMove = true;
            canTurn = true;
            canAttack = true;
            currentSpeed = roamSpeed;
            currentHealth = maxHealth;
            stunPoint = (canBeStunned) ? stunPoint : maxHealth;

            attackCollider.enabled = false;
            attackCollider.gameObject.SetActive(false);
            attackCoolDownCount = attackCooldown;
            staggerTimeCounter = staggerTime;

            if (!isTokenUser) { isTokenOwner = true; }
            attackCollider.gameObject.GetComponent<EnemyAttackCollider>()._damage = attackDamage;


            if (!isBoss) healthBackgroundUI.gameObject.SetActive(false);
        }

        public virtual void UpdateLogic()
        {
            if (isBoss) UpdateEnemyHealthBar(GetCurrentEnemyHealthProgress());

            if (isStagger) { return; }
            if (isStunned) { return; }
            distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);
            UpdateAttackCoolDown();
            _stateMachine.UpdateState();
            enemyNavAgent.isStopped = !canMove;
            enemyNavAgent.speed = currentSpeed / 10;
            if (canTurn)
            {
                LookAtTarget(transform, playerRef.transform, turnSpeed);
            }
        }

        public virtual void FixedUpdateLogic()
        {
            if (isDashing) return;
            if (isStagger) return;

            _stateMachine.FixedUpdateState();
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    //myRigidbody.velocity = Vector3.ProjectOnPlane(myRigidbody.velocity, collision.contacts[0].normal);
        //}

        public void UpdateLogicByPlayerDistance()
        {
            //if(this == null) { return; }
            distanceToPlayer = GetDistanceToPLayerIgnoreY();
            isTargetInAttackRange = (distanceToPlayer <= attackRange) ? true : false;
        }
        

        private void UpdateAttackCoolDown()
        {

            //note: call OnDonwAttack when finish attack
            if (!canAttack)
            {
                attackCoolDownCount -= Time.deltaTime;
                canAttack = (attackCoolDownCount <= 0) ? true : false;
            }
        }  //Allow attack again after a delay

        public void OnDoneAttack()
        {
            attackCoolDownCount = attackCooldown;
            canAttack = false;
            isAttacking = false;
            if (!isTokenUser) { return; }
            isTokenOwner = false;
        } //Call when finish attack
        

        #region LOGIC IMPLEMENT

        #region MOVEMENT DASH
        public void InnitDash(Vector3 dashDirection)
        {
            StartCoroutine(Dash(dashDirection, _dashDistance, _dashDuration));
        }

        public void InnitDash(Vector3 dashDirection, float dashDistance, float dashTime)
        {
            StartCoroutine(Dash(dashDirection, dashDistance, dashTime));
        }

        public IEnumerator Dash(Vector3 dashDirection, float DashDistance, float DashTime)
        {
            if (isDashing) { yield break; }

            canMove = false;
            //enemyNavAgent.updateRotation = false;           
            enemyNavAgent.ResetPath();
            //enemyNavAgent.updatePosition = false;
            isDashing = true;

            dashDirection = dashDirection.normalized;
            float dashDistance = DashDistance;
            Vector3 dashPoint = transform.position + dashDirection * dashDistance;
            LookAtTarget(dashPoint);

            Vector2 enemyPos = new Vector2(transform.position.x, transform.position.z);

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

            //if (NavMesh.SamplePosition(dashPoint, out NavMeshHit hit, 0.1f, 1))
            //{
            //    dashPoint = hit.position;
            //    dashDistance = Vector2.Distance(enemyPos, new Vector2(hit.position.x, hit.position.z)) - colliderCapsule.radius * 2;
            //}


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
            Vector3 dirToTarget = target.position - center.position;
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = rotation;
        }

        public void LookAtTarget(Vector3 target)
        {
            enemyAnimator.SetTrigger("Idle");
            Vector3 dirToTarget = GetDirectionIgnoreY(transform.position, target);
            Quaternion rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            transform.rotation = rotation;
        }
        #endregion

        #region ATTACK
        public virtual void PresetDashAttack(Vector3 DashDirection)
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

            attackCollider.gameObject.SetActive(false);
            attackCollider.enabled = false;
        }

        public void ShootProjectile(Vector3 direction)
        {
            GameObject projectile = Instantiate(_shootProjectile, _shootPoint.position, Quaternion.identity);
            if (projectile.TryGetComponent<EnemyProjectile>(out EnemyProjectile enemyProjectile))
            {
                enemyProjectile.SetUp(direction, this.gameObject);
            }

        }

        public void ShootRayAttack(Vector3 direction)
        {
            if (isStagger) return;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, layerData.hostileTargetLayer))
            {
                if (hit.transform.gameObject.CompareTag("Player"))
                {
                    hit.transform.gameObject.GetComponent<IDamageable>().TakeDamage(attackDamage);
                }
            }
        }

        #endregion

        private void HitByChargedATK(bool hit)
        {
            isChargedATK = hit;
        }


        public float GetCurrentEnemyHealthProgress()
        {
            return Mathf.InverseLerp(0f, maxHealth, currentHealth);
        }

        private void UpdateEnemyHealthBar(float value)
        {
            enemyHealthUI.fillAmount = value;
            enemyDamageReceivedUI.fillAmount = Mathf.SmoothDamp(enemyDamageReceivedUI.fillAmount, value, ref damageTakenVelocity, damageSmoothTime);
        }

        public virtual void TakeDamage(int damage)
        {
            //Debug.Log("Damage: " + damage);
            currentHealth -= damage;
            staggerThresholdCounter -= damage;
            
            PlayBloodEffect();
            PlaySplashEffect();
            enemyAnimator.SetTrigger("Hurt");
            OnEnemyDamaged?.Invoke(isChargedATK);

            if (currentHealth <= 0)
            {
                OnDeath();
            }
            if (staggerThresholdCounter <= 0)
            {
                staggerThresholdCounter = staggerThreshold;
                staggerTimeCounter = staggerTime;
                Stagger(knockbackForce);
            }
            if (currentHealth < stunPoint)
            {
                StartCoroutine(Stun());
            }

            if (isBoss) return;

            if (uiAppearCoroutine != null)
            {
                StopCoroutine(uiAppearCoroutine);
                uiAppearCoroutine = null;
            }

            if (uiAppearCoroutine == null)
                uiAppearCoroutine = StartCoroutine(UIAppear(timeLimitUI));
        }


        private IEnumerator UIAppear(float timeLimit)
        {
            float tempTime = timeLimit;
            healthBackgroundUI.gameObject.SetActive(true);

            while (tempTime > 0)
            {
                tempTime -= Time.deltaTime;
                healthBackgroundUI.transform.rotation = Camera.main.transform.rotation;

                UpdateEnemyHealthBar(GetCurrentEnemyHealthProgress());
                yield return null;
            }

            enemyDamageReceivedUI.fillAmount = GetCurrentEnemyHealthProgress();
            healthBackgroundUI.gameObject.SetActive(false);
            uiAppearCoroutine = null;
        }



















        public void DamagedByWeapon(WeaponData _weaponData)
        {
            //knockbackForce = _weaponData.knockbackForce;
        }


        public void Stagger(float knockbackStrength)
        {
            if (isStagger == true) return;
            canMove = false;
            isStagger = true;
            Vector3 knockbackDir = GetDirectionIgnoreY(playerRef.transform.position, this.transform.position).normalized;
            //rb.AddForce(knockbackDir * knockbackStrength, ForceMode.Impulse);
            StartCoroutine(ApplyKnockback(knockbackDir, knockbackStrength));
            StartCoroutine(StaggerTimeCoroutine());

        }

        IEnumerator StaggerTimeCoroutine()
        {
            while (staggerTimeCounter > 0)
            {
                staggerTimeCounter -= Time.deltaTime;
                yield return null;
            }
            //yield return new WaitForSeconds(staggerTime);

            staggerTimeCounter = staggerTime;
            isStagger = false;
            canMove = true;
        }

        private IEnumerator ApplyKnockback(Vector3 direction, float strength)
        {
            float knockbackTime = 0.2f; // Time the knockback effect lasts
            float elapsedTime = 0f;

            while (elapsedTime < knockbackTime)
            {
                transform.position += direction * (strength * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator Stun()
        {
            if (hasBeenStunned) { yield return null; }
            isStunned = true;
            hasBeenStunned = true;
            canMove = false;
            Debug.Log("Stun");
            enemyAnimator.SetTrigger("Idle");
            yield return new WaitForSeconds(stunDuration);

            isStunned = false;
            canMove = true;
            Debug.Log("End Stun");
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
            Vector3 spawnPosition = transform.position - new Vector3(0, 0.5f, 0);
            GameObject expObj = Instantiate(expPrefab, spawnPosition, Quaternion.identity);
            ExpOrb expOrb = expObj.GetComponent<ExpOrb>();
            if(expOrb == null) { Debug.LogWarning("missing script Reference");  return; } 
            if(playerRef == null) { Debug.LogWarning("missing playerRef Reference");  return; } 
            if(playerStatsRef == null) { Debug.LogWarning("missing playerRefStats Reference");  return; } 
            if(expObj == null) { Debug.LogWarning("missing obj Reference");  return; } 
            expOrb.SetExp(_dropValue, playerStatsRef, playerRef);
            if (GameManager.Instance.isPlayerDead) 
            {
                Debug.Log("PlayerDie");
                Destroy(expObj); 
            }
            OnEnemyDeaths?.Invoke(this);
            OnEnemyDeathsEvent?.Invoke(this);
            Destroy(gameObject);
        }

        #endregion

        #region HELPER

        public Vector3 GetRandomNavmeshLocationAroundSelf(float range)
        {
            //Get a random point in a circle around target
            Vector3 randDir = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y).normalized;
            Vector3 randomDirection = transform.position + randDir * range;
            NavMeshHit hitData;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hitData, range, 1))
            {
                finalPosition = hitData.position;
            }
            return finalPosition;
        }


        public Vector3 GetNavMeshLocationAroundAPoint(Vector3 center, float checkRadius)
        {
            int angle = Random.Range(0, 361);
            Vector3 location = new Vector3(center.x + checkRadius * Mathf.Cos(angle), center.y, center.z + checkRadius * Mathf.Sin(angle));
            NavMeshHit hitData;
            if (NavMesh.SamplePosition(location, out hitData, checkRadius, 1))
            {
                location = hitData.position;
            }

            return location;
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

        public Vector3 GetOffSetDirection(Vector3 direction, float offSetDegrees)
        {
            return direction = Quaternion.Euler(0, Random.Range(-offSetDegrees, offSetDegrees), 0) * direction;

        }

        public Vector3 GetPerpendicularDirectionToPLayerTarget(bool toRight = true)
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
            return new Vector3(to.x, 0, to.z) - new Vector3(from.x, 0, from.z);
        }

        public float GetDistanceToPLayer()
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);
            return distanceToPlayer;
        } //Get straight distance to player

        public float GetDistanceToPLayerIgnoreY()
        {
            Vector3 playerPos = playerRef.transform.position;
            Vector3 agentPos = transform.position;
            playerPos.y = agentPos.y;


            distanceToPlayer = Vector3.Distance(agentPos, playerPos);
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

        
        

        #region Effects

        public void PlayBloodEffect()
        {
            bloodSpatterEffect.Play();
        }

        public void PlaySplashEffect()
        {
            slashSpatterEffect.Play();
        }

        public void PlayAttackIndicatorEffect()
        {
            attackIndicatorEffect.Play();
        }
        #endregion
        
        
        
        
        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            //Gizmos.DrawLine(transform.position, GetPerpendicularVectorToTarget());
        }
    }

}

