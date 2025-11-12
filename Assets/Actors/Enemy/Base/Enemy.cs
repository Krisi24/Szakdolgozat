using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [field: SerializeField] private float MaxHealth { get; set; } = 100f;
    float CurrentHealth { get; set; }
    [field: SerializeField] private float stopDistance;
    [field: SerializeField] private float speed;
    [field: SerializeField] private float notificationDistance = 100f;
    [field: SerializeField] private Transform? patrolEndpoint;
    private Vector3 patrolEndpointPosition;
    [field: SerializeField] private LayerMask obstructionMask;
    [field: SerializeField] private LayerMask XrayMask;

    private bool isXrayOn = false;

    NavMeshAgent _navMeshAgent;
    private NavMeshPath path;
    private int currentCornerIndex = 1;
    public Animator anim { get; set; }
    public Rigidbody RB { get; set; }

    #region dectection & movement & attack
    public GameObject PlayerTarget { get; set; }
    public Transform AttackBox;
    public Vector3 playerLastPosition { get; set; }
    public bool isAggroed { get; set; }
    private string currentAnimation = "";

    #endregion

    #region stateMachine variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemyDieState DieState { get; set; }
    public EnemySearchState SearchState { get; set; }
    public EnemyPatrolState PatrolState { get; set; }
    public EnemyDistractedState DistractedState{ get; set; }
    public EnemyWalkAwayState WalkAwayState{ get; set; }
    #endregion

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine, AttackBox);
        DieState = new EnemyDieState(this, StateMachine);
        SearchState = new EnemySearchState(this, StateMachine);
        WalkAwayState = new EnemyWalkAwayState(this, StateMachine);
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");


        EnemyChaseState.OnNotifyAboutPlayer += GoAfterPlayer;
        Watch.OnNotifyAboutPlayer += GoAfterPlayer;
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
            patrolEndpointPosition = transform.position;
            PatrolState = new EnemyPatrolState(this, StateMachine, patrolEndpointPosition);
            StateMachine.Initalize(IdleState);
        }
        else
        {
            patrolEndpointPosition = patrolEndpoint.position;
            PatrolState = new EnemyPatrolState(this, StateMachine, patrolEndpointPosition);
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
        if (!isAggroed)
        {
            gameObject.GetComponent<EnemyVisionCheck>().enabled = true;
        }
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

    private void OnDestroy()
    {
        EnemyChaseState.OnNotifyAboutPlayer -= GoAfterPlayer;
        Watch.OnNotifyAboutPlayer -= GoAfterPlayer;
    }

    private void GoAfterPlayer(Vector3 playerPos, Vector3 enemyPos)
    {
        if ((Vector3.Distance(transform.position, enemyPos) > notificationDistance) ||
            StateMachine.CurrentEnemyState == DieState ||
            StateMachine.CurrentEnemyState == AttackState ||
            StateMachine.CurrentEnemyState == ChaseState
            )
        {
            return;
        }
        playerLastPosition = playerPos;
        StateMachine.ChangeState(SearchState);
    }

    public bool PlayerIsDirectlyAvailable()
    {
        NavMesh.CalculatePath(transform.position, playerLastPosition, NavMesh.AllAreas, path);
            
        if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length == 2)
        {
            return true;
        }
        return false;
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

        if (distance > 0.5f)
        {
            Vector3 direction = (pos - transform.position).normalized;
            Vector3 movement = direction * (speed / 3) * Time.deltaTime;
            
            transform.position += movement;

            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 600 * Time.deltaTime);

            return false;
        }
        return true;
    }

    public void RotateEnemyToPLayer()
    {
        Vector3 directionToTarget = PlayerTarget.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 450 * Time.deltaTime);
    }

    public bool MoveEnemyToPosSmart(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);

        if (distance > 0.5f)
        {
            Vector3 direction = (position - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;

            transform.LookAt(new Vector3(position.x, transform.position.y, position.z));

            return false;
        }
        return true;
    }

    public void SetRoute()
    {
        NavMesh.CalculatePath(transform.position, playerLastPosition, NavMesh.AllAreas, path);
        currentCornerIndex = 1;
    }

    public void SetRouteToObject(Vector3 position)
    {
        NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path);
        currentCornerIndex = 1;
    }

    public Vector3 GetNextMovePosition()
    {
        if (path.corners.Length == 0)
        {
            return transform.position;
        }

        if (currentCornerIndex >= path.corners.Length)
        {
            return path.corners[path.corners.Length - 1];
        }

        Debug.DrawRay(path.corners[currentCornerIndex], Vector3.up * 2, Color.green);

        if (Vector3.Distance(transform.position, path.corners[currentCornerIndex]) < 0.5f)
        {
            currentCornerIndex++;

            if (currentCornerIndex >= path.corners.Length)
            {
                return path.corners[path.corners.Length - 1];
            }
        }

        // Visszaadjuk az aktuális cél sarokpontot.
        return path.corners[currentCornerIndex];

    }

    public void XrayOn()
    {
        if (isXrayOn)
        {
            return;
        }
        isXrayOn = true;
        List<GameObject> allChildGameObjectsRecursive = new List<GameObject>();
        GetAllChildrenRecursive(transform, allChildGameObjectsRecursive);
        foreach (GameObject go in allChildGameObjectsRecursive)
        {
            go.layer = 8;
        }
    }

    void GetAllChildrenRecursive(Transform parent, List<GameObject> listToFill)
    {
        foreach (Transform child in parent)
        {
            listToFill.Add(child.gameObject);
            GetAllChildrenRecursive(child, listToFill);
        }
    }

    public void ChangeAnimation(string animation)
    {
        if (animation != currentAnimation)
        {
            currentAnimation = animation;
            anim.CrossFadeInFixedTime(animation, 0.2f);
        }

    }

    public void newPatrolPoint()
    {
        Vector3 startPosition = transform.position;
        float distance = 5f;

        Vector3[] surroundingPoints = new Vector3[8];
        surroundingPoints[0] = startPosition + Vector3.forward * distance;
        surroundingPoints[1] = startPosition + Vector3.back * distance;
        surroundingPoints[2] = startPosition + Vector3.right * distance;
        surroundingPoints[3] = startPosition + Vector3.left * distance;
        surroundingPoints[4] = startPosition + Vector3.forward * distance + Vector3.right * distance;
        surroundingPoints[5] = startPosition + Vector3.forward * distance + Vector3.left * distance;
        surroundingPoints[6] = startPosition + Vector3.back * distance + Vector3.right * distance;
        surroundingPoints[7] = startPosition + Vector3.back * distance + Vector3.left * distance;

        Vector3 diretion = (playerLastPosition - transform.position).normalized;

        for (int i = 0; i < surroundingPoints.Length; i++)
        {
            Debug.DrawLine(startPosition + new Vector3(0, 0.1f, 0), surroundingPoints[i] + new Vector3(0, 0.1f, 0), Color.green, 3f);
            
            if( NavMesh.CalculatePath(transform.position, surroundingPoints[i], NavMesh.AllAreas, path) && path.corners.Length == 2)
            {
                patrolEndpointPosition = surroundingPoints[i];
                break;
            }
        }

        Debug.DrawLine(patrolEndpointPosition, patrolEndpointPosition + Vector3.up, Color.green, 3f);
    }

    public Vector3 GetPatrolEndpoint()
    {
        return patrolEndpointPosition;
    }

    public void GetDistracted(GameObject distractionObject)
    {
        EnemyChaseState.OnNotifyAboutPlayer -= GoAfterPlayer;
        Watch.OnNotifyAboutPlayer -= GoAfterPlayer;
        DistractedState = new EnemyDistractedState(this, StateMachine, distractionObject);
        StateMachine.ChangeState(DistractedState);
    }

    public void GetPaidOff()
    {
        EnemyChaseState.OnNotifyAboutPlayer -= GoAfterPlayer;
        Watch.OnNotifyAboutPlayer -= GoAfterPlayer;
        StateMachine.ChangeState(WalkAwayState);
    }

    public bool IsPaidOff()
    {
        return StateMachine.CurrentEnemyState == WalkAwayState;
    }
}