using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamagable
{
    public GameObject noBoneSignal;
    public GameObject bone;
    public Transform rightHandTransform;
    private Rigidbody rb;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float rollSpeed = 7.5f;
    private float currentSpeed;
    [SerializeField] private float rotationSpeed = 1200f;
    [SerializeField] private HealthBar healthbar;
    [SerializeField] private GameObject menu;
    public Transform AttackBox;
    public float MaxHealth { get; set; } = 500f;
    public float CurrentHealth { get; set; }
    private bool isCrouches = false;
    private CapsuleCollider playerCollider;
    private Vector3 crouchPosition = new Vector3(0f, 0.65f, 0f);
    private Vector3 standingPosition = new Vector3(0f, 0.85f, 0f);
    private Animator anim;
    private string currentAnimation = "";
    private Activate interactive;
    public static event Action PlayerHasDied;

    #region Input Actions

    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction rollAction;
    private InputAction attackAction;
    private InputAction menuAction;
    private InputAction interactAction;
    private InputAction specialMoveAction;

    #endregion

    #region stateMachine variables

    public PlayerStateMachine StateMachine { get; set; }
    public MoveState MoveState { get; set; }
    public IdleState IdleState { get; set; }
    public RollState RollState { get; set; }
    public AttackState AttackState { get; set; }
    public DieState DieState { get; set; }
    public ThrowState ThrowState{ get; set; }

    #endregion

    #region MonoBehavior Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StateMachine.Initalize(IdleState);
        CurrentHealth = MaxHealth;
        currentSpeed = runSpeed;
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();

        StateMachine = new PlayerStateMachine();
        IdleState = new IdleState(this);
        RollState = new RollState(this);
        AttackState = new AttackState(this, AttackBox);
        DieState = new DieState(this);
        MoveState = new MoveState(this);
        ThrowState = new ThrowState(this, AttackBox);

        var playerMap = inputActions.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Move", true);
        crouchAction = playerMap.FindAction("Crouch", true);
        rollAction = playerMap.FindAction("Roll", true);
        attackAction = playerMap.FindAction("Attack", true);
        menuAction = playerMap.FindAction("Menu", true);
        interactAction = playerMap.FindAction("Interact", true);
        specialMoveAction = playerMap.FindAction("SpecialMove", true);
    }
    private void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
    }
    private void FixedUpdate()
    {
        StateMachine.CurrentPlayerState.PhisicsUpdate();

    }
    #endregion

    #region Input Action Methods

    private void OnMenu()
    {
        OnDisable();
        Time.timeScale = 0;
        menu.SetActive(true);
    }
    private void OnRoll()
    {
        InputLvlTwo();
        currentSpeed = rollSpeed;
        StateMachine.ChangeState(RollState);
    }
    private void OnAttack()
    {
        InputLvlOne();
        StateMachine.ChangeState(AttackState);
    }
    private void OnCrouch()
    {
        isCrouches = !isCrouches;
        if (isCrouches)
        {
            currentSpeed = crouchSpeed;
            playerCollider.center = crouchPosition;
            playerCollider.height = 1.4f;
            if (StateMachine.CurrentPlayerState == IdleState)
            {
                ChangeAnimation("Crouch Idle");
            }
            else
            {
                ChangeAnimation("Crouched Walking");
            }
        }
        else
        {
            currentSpeed = runSpeed;
            playerCollider.center = standingPosition;
            playerCollider.height = 1.9f;
            if (StateMachine.CurrentPlayerState == IdleState)
            {
                ChangeAnimation("Idle");
            }
            else
            {
                ChangeAnimation("Run");
            }
        }
    }
    private void OnMove()
    {
        InputLvlZero();
        // reset velocity
        if (rb.linearVelocity.x > 0 || rb.linearVelocity.z > 0)
        {
            rb.linearVelocity = Vector3.zero;
        }
        StateMachine.ChangeState(MoveState);
    }
    private void OnSpecialMove()
    {
        if (GameManager.instance.GetAvaiableBones() > 0)
        {
            InputLvlTwo();
            GameManager.instance.UseCollectable(CollectableType.Bone);
            StateMachine.ChangeState(ThrowState);
        }
        else
        {
            if (noBoneSignal != null)
            {
                GameObject instance = Instantiate(noBoneSignal, transform.position, Quaternion.identity);
                Destroy(instance, 2f);
            }
            else
            {
                Debug.Log("NoBoneSignal GameObject is not set! Cannot Insitatate on specialmove!");
            }
        }
    }
    private void OnInteract()
    {
        // check interactive
        if (interactive != null)
        {
            interactive.Activation();
        }

        // check pay off
        if(GameManager.instance.GetAvaiableCoins() > 0)
        {
            Collider[] surroundingCheck = Physics.OverlapSphere(transform.position, 3f, LayerMask.GetMask("Enemy"));

            if (surroundingCheck.Length == 1 &&
                surroundingCheck[0].GetComponent<Dog>() == null &&
                !surroundingCheck[0].GetComponent<Enemy>().IsPaidOff())
            {
                InputLvlTwo();
                Hud.instance.ShowPayOffIntecractionMenu(surroundingCheck[0].GetComponent<Enemy>());
            }
        }
    }
    public void SetInteractive(Activate newInteractive)
    {
        interactive = newInteractive;
    }
    public void ForgetInteractive(Activate newInteractive)
    {
        if (interactive != newInteractive) {
        } else
        {
            interactive = null;
        }
    }
    public void InputLvlZero()
    {
        OnEnable();
    }
    public void InputLvlOne()
    {
        moveAction.Disable();
        crouchAction.Disable();
        rollAction.Enable();
        attackAction.Disable();
        interactAction.Disable();
    }
    public void InputLvlTwo()
    {
        OnDisable();
    }
    void OnEnable()
    {
        moveAction.Enable();
        crouchAction.Enable();
        rollAction.Enable();
        attackAction.Enable();
        interactAction.Enable();
    }
    void OnDisable()
    {
        moveAction.Disable();
        crouchAction.Disable();
        rollAction.Disable();
        attackAction.Disable();
        interactAction.Disable();
    }

    #endregion

    public void Damage(float damageAmount)
    {
        //Debug.Log("Player Health: " + CurrentHealth + " Player Damage: " + damageAmount);
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
        {
            StateMachine.ChangeState(DieState);
        }

        if (healthbar != null)
        {
            healthbar.UpdateHealthBar(MaxHealth, CurrentHealth);
        }
        else
        {
            Debug.LogError("HealthSlider is not assigned in the Inspector!");
        }
    }

    public void ChangeAnimation(string animation)
    {
        if(animation != currentAnimation)
        {
            currentAnimation = animation;
            anim.CrossFadeInFixedTime(animation, 0.2f);
        }
    }

    #region Getters Setters
    public bool IsCrouches()
    {
        return isCrouches;
    }
    public float GetRunSpeed()
    {
        return runSpeed;
    }
    public float GetCrouchSpeed()
    {
        return crouchSpeed;
    }
    public float GetRollSpeed()
    {
        return rollSpeed;
    }

    public Vector3 GetMoveInput()
    {
        return new Vector3(moveAction.ReadValue<Vector2>().x, 0, moveAction.ReadValue<Vector2>().y);
    }

    public void ResetMaxLinearVelocity()
    {
        if (isCrouches)
        {
            rb.maxLinearVelocity = crouchSpeed;
        } else
        {
            rb.maxLinearVelocity = runSpeed;
        }
    }

    #endregion

    public void Die()
    {
        Debug.Log("You're dead!");
        PlayerHasDied?.Invoke();
    }

    public void Move(Vector3 direction)
    {
        rb.MovePosition(transform.position + direction * currentSpeed * Time.fixedDeltaTime);
        LookDirection(direction);
    }

    public void LookDirection(Vector3 velocity)
    {
        Quaternion toRotation = Quaternion.LookRotation(velocity, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }
    public void SpawnThroable()
    {
        // velocity
        Vector3 throwDirection = (transform.forward + Vector3.up).normalized;
        float force = 5f;
        GameObject newBone = Instantiate(bone, rightHandTransform.position, Quaternion.identity);
        Rigidbody newBoneRB = newBone.GetComponent<Rigidbody>();
        newBoneRB.linearVelocity = throwDirection * force;
        // spinning
        float spinForce = 5f;
        Vector3 randomTorque = UnityEngine.Random.insideUnitSphere * spinForce;
        newBoneRB.AddTorque(randomTorque, ForceMode.VelocityChange);
    }
}
