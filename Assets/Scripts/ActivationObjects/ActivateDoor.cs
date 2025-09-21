using UnityEngine;

public class ActivateDoor : MonoBehaviour, Activate
{
    public float slideDistance = 2f;

    public float slideSpeed = 3f;

    public Vector3 slideDirection = Vector3.right;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isActivated = false;
    private bool isMoving = false;

    void Start()
    {
        initialPosition = transform.position; // Elmentjük az ajtó kezdõ pozícióját
        targetPosition = initialPosition;     // Kezdetben a célpozíció is az eredeti
    }

    void Update()
    {
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
