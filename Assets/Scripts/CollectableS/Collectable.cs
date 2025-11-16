using UnityEngine;
public enum CollectableType
{
    Bone,
    Coin
}

public class Collectable : MonoBehaviour
{
    public GameObject onCollectEffect;
    public float rotationSpeedY = 90f;
    public float rotationSpeedX = 0f;
    [SerializeField] private CollectableType type = CollectableType.Bone;

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterCollectable(type);
        }
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeedY * Time.deltaTime, rotationSpeedX * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (onCollectEffect != null)
            {
                GameObject spawnedEffect = Instantiate(onCollectEffect, transform.position, transform.rotation);
                Destroy(spawnedEffect, 2f);
            }
            else
            {
                Debug.LogWarning("There is no particle effect selected in the Inpector!");
            }
            GameManager.instance.UnregisterCollectable(type);
            Destroy(gameObject);
        }
    }
}
