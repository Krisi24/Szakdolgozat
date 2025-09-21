using UnityEngine;

public class ActivateWoodDoor : MonoBehaviour, Activate
{
    [Tooltip("Hány fokkal forogjon az ajtó kinyitáskor.")]
    public float openAngle = 90f;

    [Tooltip("Milyen sebességgel forogjon az ajtó.")]
    public float rotationSpeed = 90f; // Fok/másodperc

    [Tooltip("Melyik tengely körül forogjon az ajtó? (Pl. Vector3.up a függõleges tengelyért)")]
    public Vector3 rotationAxis = Vector3.up; // Alapértelmezésben a Y tengely körül forog

    private Quaternion initialRotation; // Az ajtó eredeti forgatása
    private Quaternion targetRotation;  // A cél forgatása
    private bool isActivated = false;   // Jelzi, hogy az ajtó aktiválva van-e
    private bool isMoving = false;      // Jelzi, hogy az ajtó éppen mozog-e

    void Start()
    {
        initialRotation = transform.rotation; // Elmentjük az ajtó kezdõ forgatását
        targetRotation = initialRotation;     // Kezdetben a célforgatás is az eredeti
    }

    void Update()
    {
        // Ha az ajtó éppen mozog...
        if (isMoving)
        {
            // Forgatjuk az ajtót az aktuális forgatásából a célforgatás felé.
            // A Quaternion.RotateTowards simán forgat, amíg el nem éri a célt.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Ha az ajtó nagyon közel van a célforgatáshoz, vagy elérte azt,
            // akkor megállítjuk a mozgást.
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f) // 0.1 fok alatti eltérés
            {
                transform.rotation = targetRotation; // Pontosan beállítjuk a célforgatásra
                isMoving = false; // Befejeztük a mozgást
            }
        }
    }

    public void Activation()
    {
        if (!isActivated && !isMoving) // Csak akkor aktiváljuk, ha még nincs aktiválva és nem mozog
        {
            // Kiszámítjuk az új célforgatást: eredeti forgatás + egy extra forgatás a megadott tengely és szög körül
            targetRotation = initialRotation * Quaternion.Euler(rotationAxis * openAngle);
            isActivated = true; // Az ajtó aktiválva lett
            isMoving = true;    // Az ajtó mozog
        }
    }

    public void Deactivation()
    {
        if (isActivated && !isMoving) // Csak akkor deaktiváljuk, ha aktiválva van és nem mozog
        {
            targetRotation = initialRotation; // A célforgatás az eredeti forgatás
            isActivated = false; // Az ajtó deaktiválva lett
            isMoving = true;     // Az ajtó mozog
        }
    }
}