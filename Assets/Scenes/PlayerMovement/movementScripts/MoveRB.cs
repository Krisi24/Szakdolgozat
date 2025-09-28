using UnityEngine;

public class MoveRB : MonoBehaviour
{

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private Vector3 velocity = new Vector3(1f, 0f, 0f);

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
            rb.MovePosition(transform.position + velocity * movementSpeed * Time.fixedDeltaTime);
        }
        else if (Input.GetKey("a"))
        {
            rb.MovePosition(transform.position + -velocity * movementSpeed * Time.fixedDeltaTime);
        }
    }
}
