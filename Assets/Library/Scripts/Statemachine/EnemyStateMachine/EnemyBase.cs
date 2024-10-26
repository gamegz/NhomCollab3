using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.statemachine;
using Enemy.statemachine.States;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [HideInInspector] public GameObject playerRef;

    [SerializeField] private int maxHealth;
    [HideInInspector] public int currentHealth;

    public int attackDamage;
    public int attackRecoverTime;
    public float attackRange;

    public NavMeshAgent enemyNavAgent;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _chaseSpeed;
    [HideInInspector] public float currentSpeed;
    

    [HideInInspector] public bool hasAttackToken;
    [HideInInspector] public bool canMove;

    private int _dropValue;

    private EnemyStateMachine _stateMachine;
    private EnemyRoamState _enemyRoamState;

    public bool isTargetInAttackRange;

    public virtual void Awake()
    {       
        currentHealth = maxHealth;
    }

    public virtual void SetUpStateMachine()
    {
        _stateMachine = new EnemyStateMachine();
        _enemyRoamState = new EnemyRoamState(this, _stateMachine);
        _stateMachine.SetStartState(_enemyRoamState);
    }


    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        
    }

    public virtual void Update()
    {
        _stateMachine.UpdateState();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdateState();
    }

    public virtual void RequestAttackToken()
    {

    }

    public void Damage(int damage)
    {
        
    }

    public void OnDeath(int damage)
    {
        //Disable stuff
        //Play particle or death animation
        Destroy(gameObject);

    }
}
