using UnityEngine;

public class ActivateDoor : MonoBehaviour, Activate
{
    public float slideDistance = 2f;
    public float slideSpeed = 3f;
    public Vector3 slideDirection = Vector3.right;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isActivated = false;
    private bool isMoving = false;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void Activation()
    {
        if (!isActivated && !isMoving)
        {
            targetPosition = initialPosition + (slideDirection.normalized * slideDistance);
            isActivated = true;
            isMoving = true;
        }
    }

    public void Deactivation()
    {
        if (isActivated && !isMoving)
        {
            targetPosition = initialPosition;
            isActivated = false;
            isMoving = true;
        }
    }
}
