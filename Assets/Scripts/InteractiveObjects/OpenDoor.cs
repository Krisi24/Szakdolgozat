using System;
using UnityEngine;

public class OpenDoor : MonoBehaviour, Activate
{
    public float openAngle = 90f;
    public float rotationSpeed = 90f;
    public Vector3 rotationAxis = Vector3.up;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isActivated = false;
    private bool isMoving = false;

    private Player player;

    public static event Action ShowToolTip;
    public static event Action HideToolTip;

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
        Deactivation();
    }

    public void Deactivation()
    {
        if(isActivated && !isMoving)
        {
            targetRotation = initialRotation;
            isActivated = false;
            isMoving = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowToolTip?.Invoke();
            player = other.GetComponent<Player>();
            if (player == null) {
            } else
            {
                player.SetInteractive(this);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (player != null)
        {
            HideToolTip?.Invoke();
            player.ForgetInteractive(this);
        }
    }
}
