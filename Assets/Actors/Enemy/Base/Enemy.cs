using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;

public class Enemy : MonoBehaviour
{

    [field: SerializeField] private float MaxHealth { get; set; } = 100f;
    float CurrentHealth { get; set; }
    [field: SerializeField] private float stopDistance;
    [field: SerializeField] private float speed;
    [field: SerializeField] private float notificationDistance = 100f;
    [field: SerializeField] private Transform? patrolEndpoint;
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
    public Overlord overlord;
    public Transform AttackBox;

    public bool isFacingRight { get; set; }
    public Vector3 playerLastPosition { get; set; }
    public bool IsPlayerSeen { get; set; }
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


        EnemyChaseState.OnNotifyAboutPlayer += NotifyDetection;
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
        }
        else
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
        if (!isAggroed)
        {
            //StateMachine.ChangeState();
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
        EnemyChaseState.OnNotifyAboutPlayer -= NotifyDetection;
    }

    private void NotifyDetection(Vector3 playerPos, Vector3 enemyPos)
    {
        if ((Vector3.Distance(transform.position, enemyPos) > notificationDistance) ||
            StateMachine.CurrentEnemyState == DieState ||
            StateMachine.CurrentEnemyState == AttackState ||
            StateMachine.CurrentEnemyState == ChaseState 
            )
        {
            return;
        }
        // Debug.Log("Notify distance: " + (Vector3.Distance(transform.position, enemyPos)));
        playerLastPosition = playerPos;
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
            Vector3 movement = direction * (speed / 3) * Time.deltaTime;
            // Mozgás a célpont irányába
            transform.position += movement;

            //transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));

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
        float distance = Vector3.Distance(transform.position, playerLastPosition);

        // Ha a távolság nagyobb, mint a stopDistance, akkor mozgás
        if (distance > 0.1f)
        {
            Vector3 direction = (position - transform.position).normalized;

            // Mozgás a célpont irányába
            transform.position += direction * speed * Time.deltaTime;

            transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
            //_navMeshAgent.SetDestination(position);
            return false;
        }
        //_navMeshAgent.SetDestination(transform.position);
        return true;
    }

    public void SetRoute()
    {
        NavMesh.CalculatePath(transform.position, playerLastPosition, NavMesh.AllAreas, path);
        currentCornerIndex = 1;
        //Debug.Log("corner index: " + currentCornerIndex);
        //Debug.Log("corner lenth: " + path.corners.Length);
    }
    public Vector3 GetNextMovePosition()
    {
        if (currentCornerIndex == path.corners.Length)
        {
            return playerLastPosition;
        }

        Debug.DrawRay(playerLastPosition, Vector3.up, Color.red);
        if (Vector3.Distance(transform.position, path.corners[currentCornerIndex]) < 0.1f
            && path.corners.Length > currentCornerIndex)
        {
            //Debug.Log("corner index: " + currentCornerIndex);
            //Debug.Log("corner lenth: " + path.corners.Length);
            currentCornerIndex++;
        }
        if (currentCornerIndex < path.corners.Length)
        {
            Debug.DrawRay(path.corners[currentCornerIndex], Vector3.up, Color.green);
            return path.corners[currentCornerIndex];
        }
        return playerLastPosition;
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
            GetAllChildrenRecursive(child, listToFill); // Rekurzív hívás a gyerek gyerekeire
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
}