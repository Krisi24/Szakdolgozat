using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour, IDamagable
{

    Rigidbody rb; 
    [SerializeField] private float playerSpeed;
    private float playerSpeedCrouch;
    private float playerSpeedRun;
    private float playerSpeedRoll;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private HealthBar healthbar;
    private Animator anim;
    public Transform AttackBox;
    public float MaxHealth { get; set; } = 500f;
    public float CurrentHealth { get; set; }

    #region Input Actions

    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction rollAction;
    private InputAction attackAction;

    #endregion

    #region stateMachine variables

    public PlayerStateMachine StateMachine { get; set; }
    public MoveState IdleState { get; set; }
    public RollState RollState { get; set; }
    public AttackState AttackState { get; set; }
    public DieState DieState { get; set; }
    public HideState HideState { get; set; }

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StateMachine.Initalize(IdleState);
        CurrentHealth = MaxHealth;
        rb.maxLinearVelocity = playerSpeed;
        playerSpeedCrouch = playerSpeed / 3;
        playerSpeedRoll = playerSpeed * 2f;
        playerSpeedRun = playerSpeed;
    }


    private void Awake()
    {
        anim = GetComponent<Animator>();

        StateMachine = new PlayerStateMachine();
        IdleState = new MoveState(this, StateMachine, anim);
        RollState = new RollState(this, StateMachine, anim);
        AttackState = new AttackState(this, StateMachine, anim, AttackBox);
        DieState = new DieState(this, StateMachine, anim);
        HideState = new HideState(this, StateMachine, anim);

        var playerMap = inputActions.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Move", true);
        crouchAction = playerMap.FindAction("Crouch", true);
        rollAction = playerMap.FindAction("Roll", true);
        attackAction = playerMap.FindAction("Attack", true);
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

    public void SetSpeedToRun()
    {
        playerSpeed = playerSpeedRun;
        rb.maxLinearVelocity = playerSpeed;
    }

    public void SetSpeedToCrouch()
    {
        playerSpeed = playerSpeedCrouch;
        rb.maxLinearVelocity = playerSpeed;
    }

    public void SetSpeedToRoll()
    {
        playerSpeed = playerSpeedRoll;
        rb.maxLinearVelocity = playerSpeed;
    }

    private void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentPlayerState.PhisicsUpdate();

    }

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

    public void Die()
    {
        Debug.Log("You're dead!");
    }

    public Vector3 GetMoveInput()
    {
        return new Vector3(moveAction.ReadValue<Vector2>().x, 0, moveAction.ReadValue<Vector2>().y);
    }

    public void Move(Vector3 direction, float speed)
    {
        Vector3 velocity = direction * speed * 20000;
        rb.AddForce(velocity, ForceMode.Force);
        LookDirection(velocity);
    }

    public void LookDirection(Vector3 velocity)
    {
        Quaternion toRotation = Quaternion.LookRotation(velocity, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    public void Stop()
    {
        
    }

    public void PlayerAddForce(Vector3 force)
    {
        rb.AddForce(force * playerSpeed * 20000);
    }
}
