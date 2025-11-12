using UnityEngine;

public class Bone : MonoBehaviour
{
    private SphereCollider triggercollider;
    private Rigidbody rb;
    private bool hasDistractedDog = false;
    private Dog dog;
    void Awake()
    {
        triggercollider = GetComponent<SphereCollider>();
        triggercollider.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(rb.linearVelocity == Vector3.zero)
        {
            triggercollider.enabled = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasDistractedDog)
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            dog = other.gameObject.GetComponent<Dog>();
            if (dog != null)
            {
                hasDistractedDog = true;
                dog.Distract(gameObject);
                Destroy(this);
            }
        }
    }
}
