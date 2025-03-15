using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamagable
{

    Rigidbody rb; 
    [SerializeField] private float playerSpeed;
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

    #endregion

    private void Start()
    {
        hideAction = InputSystem.actions.FindAction("Crouch");
        rollAcion = InputSystem.actions.FindAction("Roll");
        rb = GetComponent<Rigidbody>();
        CurrentHealth = MaxHealth;
        rb.maxLinearVelocity = playerSpeed;
    }


    private void Awake()
    {
        anim = GetComponent<Animator>();
        StateMachine = new PlayerStateMachine();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        //StateMachine.CurrentPlayerState.PhisicsUpdate();
        if (rollAcion.IsPressed() )
        {
            anim.SetBool("isRoll", true);
        }
        PlayerMove();

    }

    public void Damage(float damageAmount)
    {

        
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
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * Time.deltaTime * 500 * playerSpeed;
            rb.AddForce(movement, ForceMode.VelocityChange);
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
        rb.AddForce(force * playerSpeed);
    }

    public void Roll()
    {
        anim.SetBool("isRoll", true);
        
    }
}
