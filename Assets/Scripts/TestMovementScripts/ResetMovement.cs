using UnityEngine;

public class ResetMovement : MonoBehaviour
{
    private Vector3 startingPosition;
    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = startingPosition;
        }
    }
}
