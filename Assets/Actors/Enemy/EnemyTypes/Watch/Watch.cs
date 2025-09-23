using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Watch : MonoBehaviour
{
    public float orbitSpeed = 0.5f; // Sebesség, amellyel a spotlámpa kering
    public float xAxisRadius = 5f; // Az ellipszis X tengely sugara
    public float yAxisRadius = 3f; // Az ellipszis Y tengely sugara

    [Header("Detektáló gömb beállításai")]
    public float detectionSphereRadius = 1f; // A detektáló gömb sugara
    public float detectionSphereDistance = 8f; // A detektáló gömb távolsága a spotlámpától az elõre irányban
    public LayerMask hitLayers; // Választható layerek az OverlapSphere számára
    public static event Action<Vector3, Vector3> OnNotifyAboutPlayer;

    private Light spotLight;

    // Változók a Sphere Gizmo-hoz
    private Vector3 gizmoSphereCenter;
    private float gizmoSphereRadius;
    private Collider[] detectedColliders; // Tárolja a detektált collider-eket
    private bool gizmoSphereDetected;

    void Start()
    {
        spotLight = GetComponentInChildren<Light>();

        if (spotLight == null || spotLight.type != LightType.Spot)
        {
            Debug.LogError("Spot Light Game Object is missing!");
            enabled = false;
        }
    }

    void Update()
    {
        Detection();
    }

    private void Detection()
    {
        if (spotLight == null) return;

        // Kiszámítja az aktuális idõ alapján az ellipszis pozícióját
        float angle = Time.time * orbitSpeed;
        float x = Mathf.Cos(angle) * xAxisRadius;
        float y = Mathf.Sin(angle) * yAxisRadius;

        // Beállítja a spotlámpa helyi pozícióját az ellipszis mentén
        spotLight.transform.localPosition = new Vector3(x, 0, y);

        // A detektáló gömb középpontjának számítása:
        // A spotlámpa pozíciójából kiindulva, a spotlámpa elõre irányában eltolva a detectionSphereDistance-el
        gizmoSphereCenter = spotLight.transform.position + spotLight.transform.forward * detectionSphereDistance;
        gizmoSphereRadius = detectionSphereRadius;

        // OverlapSphere-rel ellenõrzi, hogy mit érint a gömb
        // A '10' itt a maximális collider szám, amit visszaadhat
        detectedColliders = Physics.OverlapSphere(gizmoSphereCenter, gizmoSphereRadius, hitLayers);

        if (detectedColliders.Length > 0)
        {
            gizmoSphereDetected = true;
            foreach (Collider col in detectedColliders)
            {
                Debug.Log($"A detektáló gömb érintkezik: {col.name}");
                OnNotifyAboutPlayer?.Invoke(gizmoSphereCenter, gizmoSphereCenter);
            }
        }
        else
        {
            gizmoSphereDetected = false;
        }
    }

    void OnDrawGizmos()
    {
        if (!enabled || spotLight == null) return;

        // Ellipszis rajzolása (sárga)
        Gizmos.color = Color.yellow;
        Vector3 previousPoint = Vector3.zero;
        for (int i = 0; i <= 50; i++)
        {
            float angle = (float)i / 50f * 2f * Mathf.PI;
            float x = Mathf.Cos(angle) * xAxisRadius;
            float y = Mathf.Sin(angle) * yAxisRadius;
            Vector3 currentPoint = transform.position + new Vector3(x, 0, y);

            if (i > 0)
            {
                Gizmos.DrawLine(previousPoint, currentPoint);
            }
            previousPoint = currentPoint;
        }
        Gizmos.DrawLine(previousPoint, transform.position + new Vector3(xAxisRadius, 0, 0)); // Zárja be az ellipszist

        // Detektáló gömb rajzolása
        // Rajzoljuk ki mindig, hogy lássuk a pozícióját és méretét a szerkesztõben is
        if (gizmoSphereDetected)
        {
            // Ha valamit detektált, zöld színnel rajzoljuk a gömböt
            Gizmos.color = Color.green;
        }
        else
        {
            // Ha nem detektált semmit, piros színnel rajzoljuk a gömböt
            Gizmos.color = Color.red;
        }
        // Rajzoljunk egy tömör gömböt, ha van detektálás, különben csak drótvázat
        if (Application.isPlaying && gizmoSphereDetected)
        {
            Gizmos.DrawSphere(gizmoSphereCenter, gizmoSphereRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(gizmoSphereCenter, gizmoSphereRadius);
        }


        // Ha volt detektálás, rajzoljuk ki a detektált objektumokat is
        if (Application.isPlaying && gizmoSphereDetected && detectedColliders != null)
        {
            Gizmos.color = Color.blue; // Kék színnel jelöljük a detektált objektumokat
            foreach (Collider col in detectedColliders)
            {
                if (col != null)
                {
                    Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
                }
            }
        }
    }
}
