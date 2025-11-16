using UnityEngine;

public class ActivateWoodDoor : MonoBehaviour, Activate
{
    // calculate in degree
    public float openAngle = 90f;
    public float rotationSpeed = 90f;
    public Vector3 rotationAxis = Vector3.up;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isActivated = false;
    private bool isMoving = false;

    void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = initialRotation;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isMoving = false;
            }
        }
    }

    public void Activation()
    {
        if (!isActivated && !isMoving)
        {
            targetRotation = initialRotation * Quaternion.Euler(rotationAxis * openAngle);
            isActivated = true;
            isMoving = true;
        }
    }

    public void Deactivation()
    {
        if (isActivated && !isMoving)
        {
            targetRotation = initialRotation;
            isActivated = false;
            isMoving = true;
        }
    }
}