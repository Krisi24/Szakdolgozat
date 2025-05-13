using System.IO;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{

    [field: SerializeField] private float MaxHealth { get; set; } = 100f;
    float CurrentHealth { get; set; }
    [field: SerializeField] private float stopDistance;
    [field: SerializeField] private float speed;
    [field: SerializeField] private Transform? patrolEndpoint;
    [field: SerializeField] private LayerMask obstructionMask;

    NavMeshAgent _navMeshAgent;
    private NavMeshPath path;
    private int currentCornerIndex = 1;
    public Animator anim { get; set; }
    public Rigidbody RB { get; set; }

    #region dectection & movement & attack
    public GameObject PlayerTarget { get; set; }
    public Overlord overlord;
    public Transform AttackBox;

    public bool isFacingRight { get; set; }
    public Vector3 playerLastPosition { get; set; }
    public Vector3 notifyPos { get; set; }
    public bool IsPlayerSeen { get; set; }
    public bool isAggroed { get; set; }

    public float detectionRange = 10f;
    public float avoidStrength = 5f;
    public float obstacleCheckDistance = 2f;

    #endregion

    #region stateMachine variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemyDieState DieState { get; set; }
    public EnemySearchState SearchState { get; set; }
    public EnemyPatrolState PatrolState { get; set; }
    #endregion

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine, AttackBox);
        DieState = new EnemyDieState(this, StateMachine);
        SearchState = new EnemySearchState(this, StateMachine);
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        overlord = GameObject.FindFirstObjectByType<Overlord>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
        RB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        if (_navMeshAgent == null)
        {
            Debug.Log("NavMeshAgent is missing!");
        }

        if (patrolEndpoint == null)
        {
            StateMachine.Initalize(IdleState);
        } else
        {
            PatrolState = new EnemyPatrolState(this, StateMachine, patrolEndpoint);
            StateMachine.Initalize(PatrolState);
        }
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }
    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhisicsUpdate();
    }

    public void Damage(float damageAmount)
    {
        //Debug.Log("Health: " + CurrentHealth + "_Damage: " + damageAmount);
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
        {
            StateMachine.ChangeState(DieState);
        }
        
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void NotifyDetection(Vector3 pos)
    {
        notifyPos = pos;
        StateMachine.ChangeState(SearchState);
    }

    // Returns true if stops
    public bool MoveEnemyToPlayer()
    {
        float distance = Vector3.Distance(transform.position, PlayerTarget.transform.position);

        // Ha a távolság nagyobb, mint a stopDistance, akkor mozgás
        if (distance > stopDistance)
        {
            // A célpont irányának kiszámítása
            Vector3 direction = (PlayerTarget.transform.position - transform.position).normalized;

            // Mozgás a célpont irányába
            transform.position += direction * speed * Time.deltaTime;

            transform.LookAt(new Vector3(PlayerTarget.transform.position.x, transform.position.y, PlayerTarget.transform.position.z));

            return false;
        }
        return true;
    }

    public bool MoveEnemyToLastSeenPos()
    {
        float distance = Vector3.Distance(transform.position, playerLastPosition);

        // Ha a távolság nagyobb, mint a stopDistance, akkor mozgás
        if (distance > 0.1f)
        {
            // A célpont irányának kiszámítása
            Vector3 direction = (playerLastPosition - transform.position).normalized;

            // Mozgás a célpont irányába
            transform.position += direction * speed * Time.deltaTime;

            transform.LookAt(new Vector3(playerLastPosition.x, transform.position.y, playerLastPosition.z));

            return false;
        }
        return true;
    }

    public bool MoveEnemyToPos(Vector3 pos)
    {
        float distance = Vector3.Distance(transform.position, pos);

        // Ha a távolság nagyobb, mint a stopDistance, akkor mozgás
        if (distance > 2f)
        {
            // A célpont irányának kiszámítása
            Vector3 direction = (pos - transform.position).normalized;
            Vector3 movement = direction * speed * Time.deltaTime;
            // Mozgás a célpont irányába
            transform.position += movement;

            //transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));

            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 1200 * Time.deltaTime);

            return false;
        }
        return true;
    }


    public void SetRoute(Vector3 position)
    {
        NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path);
    }
    public bool MoveEnemyToPosSmart(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, PlayerTarget.transform.position);

        // Ha a távolság nagyobb, mint a stopDistance, akkor mozgás
        if (distance > stopDistance)
        {
            NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path);



            Vector3 direction = (PlayerTarget.transform.position - transform.position).normalized;

            // Mozgás a célpont irányába
            transform.position += direction * speed * Time.deltaTime;

            transform.LookAt(new Vector3(PlayerTarget.transform.position.x, transform.position.y, PlayerTarget.transform.position.z));
            //_navMeshAgent.SetDestination(position);
            return false;
        }
        //_navMeshAgent.SetDestination(transform.position);
        return true;
    }
}