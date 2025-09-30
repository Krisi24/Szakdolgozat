using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private GameObject onCollectEffect;
    [SerializeField] private float rotationSpeedY = 90f;
    [SerializeField] private float rotationSpeedX = 0f;
    [SerializeField] private bool countable = false;

    public static event Action OnAwake;
    public static event Action OnCollect;

    private void Awake()
    {
        if (countable)
        {
            OnAwake?.Invoke();
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
                Destroy(spawnedEffect, 2f); // destroy particle after 2 sec
            }
            else
            {
                Debug.LogWarning("There is no particle effect selected in the Inpector!");
            }

            if (countable)
            {
                OnCollect?.Invoke();
            }
            Destroy(gameObject);
        }
    }
}
