using UnityEngine;

public class ActivateDoor : MonoBehaviour, Activate
{
    [Tooltip("Milyen messzire csússzon az ajtó az eredeti pozíciójától.")]
    public float slideDistance = 2f;

    [Tooltip("Milyen sebességgel csússzon az ajtó.")]
    public float slideSpeed = 3f;

    [Tooltip("Melyik tengely mentén csússzon az ajtó? X = oldalra, Y = fel/le, Z = elõre/hátra.")]
    public Vector3 slideDirection = Vector3.right; // Alapértelmezésben jobbra csúszik

    private Vector3 initialPosition; // Az ajtó eredeti pozíciója
    private Vector3 targetPosition;  // A célpozíció, ahova csúsznia kell
    private bool isActivated = false; // Jelzi, hogy az ajtó aktiválva van-e
    private bool isMoving = false;    // Jelzi, hogy az ajtó éppen mozog-e

    void Start()
    {
        initialPosition = transform.position; // Elmentjük az ajtó kezdõ pozícióját
        targetPosition = initialPosition;     // Kezdetben a célpozíció is az eredeti
    }

    void Update()
    {
        // Ha az ajtó éppen mozog...
        if (isMoving)
        {
            // Mozgatjuk az ajtót az aktuális pozíciójából a célpozíció felé.
            // A Vector3.MoveTowards simán mozgat, amíg el nem éri a célt.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);

            // Ha az ajtó nagyon közel van a célpozícióhoz, vagy elérte azt,
            // akkor megállítjuk a mozgást.
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition; // Pontosan beállítjuk a célpozícióra
                isMoving = false; // Befejeztük a mozgást
            }
        }
    }

    public void Activation()
    {
        if (!isActivated && !isMoving) // Csak akkor aktiváljuk, ha még nincs aktiválva és nem mozog
        {
            // Kiszámítjuk az új célpozíciót: eredeti pozíció + a csúszás iránya * csúszás távolsága
            targetPosition = initialPosition + (slideDirection.normalized * slideDistance);
            isActivated = true; // Az ajtó aktiválva lett
            isMoving = true;    // Az ajtó mozog
        }
    }

    public void Deactivation()
    {
        if (isActivated && !isMoving) // Csak akkor deaktiváljuk, ha aktiválva van és nem mozog
        {
            targetPosition = initialPosition; // A célpozíció az eredeti pozíció
            isActivated = false; // Az ajtó deaktiválva lett
            isMoving = true;     // Az ajtó mozog
        }
    }
}
