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
    [field: SerializeField] private LayerMask enemyLayer;
    [field: SerializeField] private LayerMask XrayMask;

    private bool isXrayOn = false;

    NavMeshAgent _navMeshAgent;
    private NavMeshPath path;
    private int currentCornerIndex = 1;
    public Animator anim { get; set; }
    public Rigidbody RB { get; set; }

    #region avoid

    private Vector3 offsetHeight = new Vector3(0f, 0.5f, 0f);
    private Vector3 offsetRight;
    private Vector3 offsetLeft;

    private float avoidDistance = 1f;

    #endregion

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
            StateMachine.CurrentEnemyState == WalkAwayState ||
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
    // moves enemy to player except there is another enemy is in front of it
    public bool MoveEnemyToPlayer()
    {
        float distance = Vector3.Distance(transform.position, PlayerTarget.transform.position);
        Vector3 pointToLookAt = new Vector3(PlayerTarget.transform.position.x, transform.position.y, PlayerTarget.transform.position.z);

        if (distance > stopDistance)
        {
            // enemy have to move closer
            if (AvoidEnemies())
            {
                // enemy didnt need to avoid enemy
                Vector3 direction = (PlayerTarget.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                transform.LookAt(pointToLookAt);
            }
            return false;
        }
        return true;
    }

    // true -> there is no enemy ahead
    // false -> there is enemy ahead, this func. had to avoid enemy
    private bool AvoidEnemies()
    {
        offsetRight = transform.right * 0.25f;
        offsetLeft = -transform.right * 0.25f;
        Vector3 checkStartPos = transform.position + offsetHeight;
        // check forward
        if (Physics.Raycast(transform.position + offsetHeight + offsetRight, transform.forward, avoidDistance, enemyLayer) ||
            Physics.Raycast(transform.position + offsetHeight + offsetLeft, transform.forward, avoidDistance, enemyLayer))
        {
            return true;
        }

        float radius = avoidDistance;
        float angleStep = 15f;
        float fullAngle = 180f;
        List<Vector3> pointList = GetPointsAroundEnemyForward(angleStep, radius, fullAngle); // 90fok -> 6 point
        // point order at 90 degree -> 3, 4, 2, 5, 1, 6

        int[] order = GetCheckOrderForAvoidEnemies((int)angleStep, (int)fullAngle);
        
        for (int i = 0; i < order.Length; i++) {
            if(CheckPointForEnemy(checkStartPos, pointList[order[i]] + offsetHeight)) continue;
            else
            {
                MoveEnemyToPos(pointList[order[i]]);
                return false;
            }
        }
        // Todo.. Refactor
        return true;
    }

    // false -> there is no enemy
    // true -> there is a enemy
    private bool CheckPointForEnemy(Vector3 startPos, Vector3 pointToCheck)
    {
        Vector3 direction = (startPos - pointToCheck).normalized;
        if(!Physics.Raycast(startPos, direction, avoidDistance, enemyLayer))
        {
            Debug.DrawLine(startPos, pointToCheck * avoidDistance, Color.green, 3f);
            return false;
        }
        Debug.DrawLine(startPos, pointToCheck * avoidDistance, Color.red, 2f);
        return true;
    }
    private List<Vector3> GetPointsAroundEnemyForward(float angleStep, float radius, float checkAngle = 180f)
    {
        // result list
        List<Vector3> pointList = new List<Vector3>();
        // auxiliary variables
        float startAngle = -(checkAngle / 2) + transform.eulerAngles.y;
        float endAngle = (checkAngle / 2) + transform.eulerAngles.y;

        for (float i = startAngle; i < endAngle; i += angleStep)
        {
            float radian = i * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * radius;
            float z = Mathf.Sin(radian) * radius;

            pointList.Add(transform.position + new Vector3(x, 0, z));
            Debug.DrawRay(transform.position + new Vector3(x, 0, z), Vector3.up);
        }

        return pointList;
    }

    private int[] GetCheckOrderForAvoidEnemies(int angleStep, int fullAngle)
    {
        int listLength = fullAngle / angleStep;
        int[] order = new int[listLength];

        for (int i = 0; i < listLength; i++) {
            if(i % 2 == 0)  order[i] = (listLength / 2) + (i / 2);
            else            order[i] = (listLength / 2) - ((i / 2) + 1);
        }
        return order;
    }

    public bool MoveEnemyToLastSeenPos()
    {
        float distance = Vector3.Distance(transform.position, playerLastPosition);
        if (distance > 0.1f)
        {
            Vector3 direction = (playerLastPosition - transform.position).normalized;
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

    // false -> turn left 
    // true -> turn right
    public void TurnLeftOrRight(bool right)
    {
        transform.Rotate(0, right ? -450 * Time.deltaTime : 450 * Time.deltaTime, 0);
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

    // distraction
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