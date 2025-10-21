using UnityEngine;
using UnityEngine.InputSystem;

public class TransformTranslate : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Vector3 velocity = new Vector3(1f, 0f, 0f);

    private Collider playerCollider;

    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        Vector3 tmpVelocity = velocity;
        // simple collision detection
        if(Physics.Raycast(new Vector3(playerCollider.bounds.max.x, playerCollider.bounds.center.y, playerCollider.bounds.center.z), transform.right, 0.15f, wallMask))
        {
            tmpVelocity.x = 0f;
        }

        // move
        if (Input.GetKey("d"))
        {
            transform.Translate(tmpVelocity * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey("a"))
        {
            transform.Translate(-velocity * moveSpeed * Time.deltaTime);
        }
        
        //transform.position += (Vector3)velocity * moveSpeed * Time.deltaTime;
        //transform.position = Vector3.Lerp(transform.position, transform.position + velocity * moveSpeed, Time.deltaTime);
    }
}
