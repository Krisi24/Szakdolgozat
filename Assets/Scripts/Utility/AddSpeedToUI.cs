using UnityEngine;

public class AddSpeedToUI : MonoBehaviour
{
    public Vector2 moveDirection = new Vector2 (0, 1);
    public float speed = 100f;

    private RectTransform rectTransform;

     void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if(rectTransform == null)
        {
            Debug.Log("AddSeepToUI script couldnt find the Rectransform component!");
            Destroy(gameObject);
        }

        moveDirection = moveDirection.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition += moveDirection * speed * Time.deltaTime;
    }
}
