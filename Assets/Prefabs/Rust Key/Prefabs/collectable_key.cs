using UnityEngine;

public class collectable_key : MonoBehaviour
{
    public float rotationSpeed = 90f;
    [SerializeField] public MonoBehaviour activatableTarget = null;
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
        transform.Rotate(0, rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // MOST MÁR AZ activationInterface-et használjuk, ami már ellenõrzötten létezik (vagy null)
            if (activationInterface != null)
            {
                activationInterface.Activation(); // Meghívjuk az Activate interface metódusát
                Debug.Log("Collectable Key: Kulcs felvéve, 'Activation()' meghívva az " + activatableTarget.name + " objektumon keresztül.");
            }
            else
            {
                // Ez az ág akkor fut le, ha a Start() során nem találtunk interface-t,
                // vagy ha valamilyen okból mégis null maradt a referencia (pl. dinamikus törlés).
                Debug.LogError("Collectable Key: 'Player' felvette a kulcsot, de az 'activatableTarget' nem implementálja az 'Activate' interface-t, vagy az objektum null! Nem sikerült az aktiválás.");
            }

            gameObject.SetActive(false);
        }
    }
}
