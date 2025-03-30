using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour, IDamagable
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

    InputAction hideAction;
    InputAction rollAcion;

    #region stateMachine variables

    public PlayerStateMachine StateMachine { get; set; }
    public IdleState IdleState { get; set; }
    public RollState RollState { get; set; }
    public AttackState AttackState { get; set; }
    public DieState DieState { get; set; }
    public HideState HideState { get; set; }

    #endregion

    private void Start()
    {
        hideAction = InputSystem.actions.FindAction("Crouch");
        rollAcion = InputSystem.actions.FindAction("Roll");
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
        IdleState = new IdleState(this, StateMachine, anim);
        RollState = new RollState(this, StateMachine, anim);
        AttackState = new AttackState(this, StateMachine, anim, AttackBox);
        DieState = new DieState(this, StateMachine, anim);
        HideState = new HideState(this, StateMachine, anim);
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
        if (rollAcion.WasPressedThisFrame() && RollState.IsUsable())
        {
            StateMachine.ChangeState(RollState);
        }
        else if (Input.GetMouseButtonDown(0) && AttackState.IsUsable())
        {
            if(StateMachine.CurrentPlayerState == HideState)
            {
                StateMachine.ChangeState(IdleState);
            }
            anim.SetBool("isMoving", false);
            StateMachine.ChangeState(AttackState);
        } else if (hideAction.WasPressedThisFrame())
        {
            if ( StateMachine.CurrentPlayerState == HideState)
            {
                StateMachine.ChangeState(IdleState);
            }
            else
            {
                StateMachine.ChangeState(HideState);
            }
        }
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

    public void PlayerMove()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            anim.SetBool("isMoving", true);
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * playerSpeed * 20000;
            rb.AddForce(movement);
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            
            anim.SetBool("isMoving", false);
        }
    }

    public void PlayerAddForce(Vector3 force)
    {
        rb.AddForce(force * playerSpeed * 20000);
    }
}
