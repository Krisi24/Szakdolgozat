using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

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

    #region avoidance

    //private Transform playerTarget;
    private LayerMask avoidanceMask;
    private float avoidDistance = 1.5f;
    private int numViewDirections = 16;
    private float viewAngle = 180f;
    private Vector3 hightOffset = Vector3.up;

    Transform playerTransform;

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

        //aviodance
        avoidanceMask = obstructionMask + enemyLayer;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

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
        float distance = Vector3.Distance(transform.position, playerLastPosition);

        if ( stopDistance < distance)
        {
            RunToPosition(playerLastPosition);
            /*
            // best direction in context
            Vector3 bestDirection = GetSteeringDirection();
            //Debug purposes
            Debug.DrawRay(bestDirection + transform.position, Vector3.up, Color.red);
            DebugGetSteeringDirection();
            // move enemy to the best direction
            RunToPosition(transform.position + bestDirection);
            */
            return false;
        }
        return true;
    }

    private Vector3 GetSteeringDirection()
    {
        // direction & danger
        List<float> danger = new List<float>();
        List<Vector3> directions = new List<Vector3>();
        // angle variables
        float angleStep = viewAngle / (numViewDirections);
        float startAngle = -viewAngle / 2;
        Vector3 startPos = transform.position + hightOffset;

        playerLastPosition = playerTransform.position;


        // add default angle first (direction of the player)
        // directions
        Vector3 playerDir = (playerLastPosition - transform.position).normalized;
        directions.Add(playerDir);

        // calculate dangerValue
        float currentDanger = 0;
        if (Physics.Raycast(startPos, playerDir, out RaycastHit hitf, avoidDistance, avoidanceMask))
        {
            // The closer the wall, the greater the danger
            currentDanger = 1 - (hitf.distance / avoidDistance);
        }
        if(currentDanger == 0)
        {
            float currentDistance = Vector3.Distance((transform.position + playerDir), playerLastPosition);
            currentDanger -= currentDistance;
        }
        danger.Add(currentDanger);



        // calculate all other angle
        for (int i = 0; i < numViewDirections; i++)
        {
            // directions
            float angle = startAngle + i * angleStep;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            directions.Add(dir);

            // calculate dangerValue
            if (Physics.Raycast(startPos, dir, out RaycastHit hit, avoidDistance, avoidanceMask))
            {
                // The closer the wall, the greater the danger
                currentDanger = 1 - (hit.distance / avoidDistance);
            }
            if (currentDanger == 0)
            {
                float currentDistance = Vector3.Distance((transform.position + playerDir), playerLastPosition);
                currentDanger -= 100 - currentDistance;
            }
            danger.Add(currentDanger);
        }


        // choose the less dangerous direction
        // chosse the first, what is the direction of the player
        Vector3 bestDirection = directions[0];
        float lowestDanger = danger[0];

        for (int i = 1; i < directions.Count; i++)
        {
            if (danger[i] < lowestDanger)
            {
                lowestDanger = danger[i];
                bestDirection = directions[i];
            }
        }

        return bestDirection;
    }

    // red line if hit something
    // geen line if it is a good direction
    // blue line if it is the best direction
    private void DebugGetSteeringDirection()
    {
        float angleStep = viewAngle / (numViewDirections - 1);
        float startAngle = -viewAngle / 2;
        Vector3 startPos = transform.position + hightOffset;

        for (int i = 0; i < numViewDirections; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(startPos, dir, out RaycastHit hit, avoidDistance, avoidanceMask))
            {
                Debug.DrawLine(startPos, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(startPos, startPos + dir, Color.green);
            }
        }
        Debug.DrawLine(startPos, startPos + GetSteeringDirection(), Color.blue);
    }

    private void RunToPosition(Vector3 goalPosition)
    {
        Vector3 direction = (goalPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        //transform.LookAt(goalPosition);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 450 * Time.deltaTime);
    }

    // true -> there is no enemy ahead
    // false -> there is enemy ahead, this func. had to avoid enemy
    private bool AvoidEnemies()
    {
        Vector3 offsetHeight = Vector3.up;

        Vector3 checkStartPos = transform.position + offsetHeight;

        float radius = avoidDistance;
        float angleStep = 15f;
        float fullAngle = 360f;
        List<Vector3> pointList = GetPointsAroundEnemyForward(angleStep, radius, fullAngle);
        int[] order = GetCheckOrderForAvoidEnemies((int)angleStep, (int)fullAngle);
        
        for (int i = 0; i < order.Length; i++) {
            if(CheckPointForEnemy(checkStartPos, pointList[order[i]] + offsetHeight)) continue;
            else
            {
                RunToPosition(pointList[order[i]]);
                break;
            }
        }
        // Todo.. Refactor
        return false;
    }

    // false -> there is no enemy
    // true -> there is a enemy
    private bool CheckPointForEnemy(Vector3 startPos, Vector3 pointToCheck)
    {
        Vector3 direction = (startPos - pointToCheck).normalized;
        if(!Physics.Raycast(startPos, direction, avoidDistance, enemyLayer))
        {
            Debug.DrawLine(startPos, pointToCheck, Color.green, 2f);
            return false;
        }
        Debug.DrawLine(startPos, pointToCheck, Color.red, 5f);
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
            //Debug.DrawRay(transform.position + new Vector3(x, 0, z), Vector3.up, Color.white, 1f);
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