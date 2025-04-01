using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour, IDamagable
{

    private Rigidbody rb; 
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float rollSpeed = 9f;
    [SerializeField] private float rotationSpeed = 1200f;
    [SerializeField] private HealthBar healthbar;
    public Transform AttackBox;
    public float MaxHealth { get; set; } = 500f;
    public float CurrentHealth { get; set; }
    private bool isCrouches = false;

    private Animator anim;
    private string currentAnimation = "";


    #region Input Actions

    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction rollAction;
    private InputAction attackAction;

    #endregion

    #region stateMachine variables

    public PlayerStateMachine StateMachine { get; set; }
    public MoveState MoveState { get; set; }
    public IdleState IdleState { get; set; }
    public RollState RollState { get; set; }
    public AttackState AttackState { get; set; }
    public DieState DieState { get; set; }

    #endregion

    #region MonoBehavior Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StateMachine.Initalize(IdleState);
        CurrentHealth = MaxHealth;
        rb.maxLinearVelocity = runSpeed;
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();

        StateMachine = new PlayerStateMachine();
        IdleState = new IdleState(this);
        RollState = new RollState(this);
        AttackState = new AttackState(this, AttackBox);
        DieState = new DieState(this);
        MoveState = new MoveState(this);

        var playerMap = inputActions.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Move", true);
        crouchAction = playerMap.FindAction("Crouch", true);
        rollAction = playerMap.FindAction("Roll", true);
        attackAction = playerMap.FindAction("Attack", true);
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
    private void OnRoll()
    {
        InputLvlTwo();
        rb.maxLinearVelocity = rollSpeed;
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
            rb.maxLinearVelocity = crouchSpeed;
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
            rb.maxLinearVelocity = runSpeed;
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
        StateMachine.ChangeState(MoveState);
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
    }
    void OnDisable()
    {
        moveAction.Disable();
        crouchAction.Disable();
        rollAction.Disable();
        attackAction.Disable();
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
    public Vector3 GetMoveInput()
    {
        return new Vector3(moveAction.ReadValue<Vector2>().x, 0, moveAction.ReadValue<Vector2>().y);
    }
    public float GetCrouchSpeed()
    {
        return crouchSpeed;
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
    }

    public void Move(Vector3 direction)
    {
        Vector3 velocity = direction * runSpeed;
        rb.AddForce(velocity, ForceMode.VelocityChange);
        LookDirection(velocity);
    }

    public void LookDirection(Vector3 velocity)
    {
        Quaternion toRotation = Quaternion.LookRotation(velocity, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

}
