using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour, IDamagable, IEnemyMovable
{

    [field: SerializeField] private float MaxHealth { get; set; } = 100f;
    [field: SerializeField] private float stopDistance;
    [field: SerializeField] private float speed;
    [field: SerializeField] private Transform? patrolEndpoint;
    [field: SerializeField] private LayerMask obstructionMask;
    public GameObject PlayerTarget { get; set; }
    public Animator anim { get; set; }
    public Transform AttackBox;
    float CurrentHealth { get; set; }
    float IDamagable.MaxHealth { get => MaxHealth; set => MaxHealth = value; }
    float IDamagable.CurrentHealth { get => CurrentHealth; set => CurrentHealth = value; }
    public Rigidbody RB { get; set; }
    public bool isFacingRight { get; set; }

    public Vector3 playerLastPosition { get; set; }
    public Vector3 notifyPos { get; set; }
    public bool IsPlayerSeen { get; set; }


    #region stateMachine variables

    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemyDieState DieState { get; set; }
    public EnemySearchState SearchState { get; set; }
    public EnemyPatrolState PatrolState { get; set; }
    public EnemyNotifiedState NotifiedState { get; set; }
    public bool isAggroed { get; set; }


    public float detectionRange = 10f;
    public float avoidStrength = 5f;
    public float obstacleCheckDistance = 2f;

    public Overlord overlord;

    #endregion

    #region IdleVariables

    public float RandomMovementRange = 5f;
    public float RandomMovementSpeed = 3f;

    #endregion

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine, AttackBox);
        DieState = new EnemyDieState(this, StateMachine);
        SearchState = new EnemySearchState(this, StateMachine);
        NotifiedState = new EnemyNotifiedState(this, StateMachine);
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        overlord = GameObject.FindFirstObjectByType<Overlord>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
        RB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

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

    public void NotifyDetection(Vector3 pos)
    {
        notifyPos = pos;
        StateMachine.ChangeState(NotifiedState);
    }

    public bool MoveEnemyToPosSmart(Vector3 position)
    {
        if(Vector3.Distance(transform.position, position) < stopDistance)
        {
            return true;
        }
        Vector3 enemypos = new Vector3(0f, 1.5f, 0f) + transform.position;

        Vector3 toPlayer = (position - transform.position).normalized;
        Vector3 finalDirection = toPlayer;

        // Obstacle avoidance ray
        if (Physics.Raycast(enemypos, toPlayer, out RaycastHit hit, obstacleCheckDistance, obstructionMask))
        {
            // Ha falat lát, akkor próbál oldalra kitérni
            Vector3 obstacleNormal = hit.normal;
            Vector3 avoidDirection = Vector3.Cross(obstacleNormal, Vector3.up).normalized;

            // Döntsük el, melyik irányba próbáljunk kitérni
            Vector3 rightTry = Quaternion.AngleAxis(45, Vector3.up) * toPlayer;
            Vector3 leftTry = Quaternion.AngleAxis(-45, Vector3.up) * toPlayer;

            bool rightFree = !Physics.Raycast(enemypos, rightTry, obstacleCheckDistance, obstructionMask);
            bool leftFree = !Physics.Raycast(enemypos, leftTry, obstacleCheckDistance, obstructionMask);

            if (rightFree)
                avoidDirection = rightTry;
            else if (leftFree)
                avoidDirection = leftTry;

            finalDirection = Vector3.Lerp(toPlayer, avoidDirection, 0.7f).normalized;
        }

        // Mozgatás
        transform.position += finalDirection * speed * Time.fixedDeltaTime;

        // Forgatás a mozgás irányába
        if (finalDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
        return false;
    }
}