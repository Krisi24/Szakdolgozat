using UnityEngine;

public class AddForceRB : MonoBehaviour
{
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private Vector3 velocity = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector3 velocityBack = new Vector3(-1f, 0f, 0f);

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (Input.GetKey("d"))
        {
            rb.AddForce(velocity * acceleration, ForceMode.Force);
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else if (Input.GetKey("a"))
        {
            rb.AddForce(velocityBack * acceleration, ForceMode.Force);
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            rb.AddForce(rb.linearVelocity * -acceleration, ForceMode.Force);
        }
    }
}
