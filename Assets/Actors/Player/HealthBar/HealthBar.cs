using UnityEngine;
using UnityEngine.UI;

public class HealthBar: MonoBehaviour
{
    [SerializeField] private Image healthbarSprite;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        transform.LookAt(Camera.main.transform);
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        float fillValue = (float) currentHealth / maxHealth;
        healthbarSprite.fillAmount = fillValue;
        //Debug.Log("Fill Amount: " + fillValue);
    }
}
