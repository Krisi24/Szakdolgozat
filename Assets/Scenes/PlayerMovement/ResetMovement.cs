using UnityEngine;
using UnityEngine.InputSystem;

public class ResetMovement : MonoBehaviour
{
    private Vector3 startingPosition;
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = startingPosition;
        }
    }
}
