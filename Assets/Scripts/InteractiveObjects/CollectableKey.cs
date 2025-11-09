using UnityEngine;

public class CollectableKey : MonoBehaviour
{
    public GameObject onCollectEffect;
    public float rotationSpeedY = 90f;
    public float rotationSpeedX = 0f;
    public MonoBehaviour activatableTarget = null;
    private Activate activationInterface = null;

    void Start()
    {
        if (activatableTarget != null)
        {
            activationInterface = activatableTarget.GetComponent<Activate>();
            if (activationInterface == null)
            {
                Debug.LogWarning("Collectable Key: Az 'activatableTarget' GameObjecten (" + activatableTarget.name + ") nincs 'Activate' interface implementációja. A kulcs inaktívvá tétele.");
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Collectable Key: Nincs beállítva az 'activatableTarget' a " + gameObject.name + " objektumon. A kulcs inaktívvá tétele.");
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeedY * Time.deltaTime, rotationSpeedX * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (activationInterface != null)
            {
                activationInterface.Activation();
            }
            else
            {
                Debug.LogError("Collectable Key: Player picked up the key, but the activation has failed!");
            }
            if (onCollectEffect != null)
            {
                GameObject spawnedEffect = Instantiate(onCollectEffect, transform.position, transform.rotation);
                Destroy(spawnedEffect, 2f);
            }
            else
            {
                Debug.LogWarning("There is no particle effect selected in the Inpector!");
            }
            Destroy(gameObject);
        }
    }


}
