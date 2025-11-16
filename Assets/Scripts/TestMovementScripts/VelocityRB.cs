using UnityEngine;

public class VelocityRB : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey("d"))
        {
            rb.linearVelocity = Vector3.right * moveSpeed;
        }
        else if (Input.GetKey("a"))
        {
            rb.linearVelocity = Vector3.left * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
