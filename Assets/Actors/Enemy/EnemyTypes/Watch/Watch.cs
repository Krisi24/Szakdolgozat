using System;
using UnityEngine;

public class Watch : MonoBehaviour
{
    public float orbitSpeed = 0.5f;
    public float xAxisRadius = 5f;
    public float yAxisRadius = 3f;

    [Header("Detektáló gömb beállításai")]
    public float detectionSphereRadius = 1f;
    public float detectionSphereDistance = 8f;
    public LayerMask hitLayers;
    public static event Action<Vector3, Vector3> OnNotifyAboutPlayer;

    [Header("Enemy Spawner")]
    public GameObject enemyPrefab;
    public GameObject spawnPointObj;
    public int spawnCount = 0;

    private Light spotLight;

    // Sphere Gizmo
    private Vector3 gizmoSphereCenter;
    private float gizmoSphereRadius;
    private Collider[] detectedColliders;
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

        float angle = Time.time * orbitSpeed;
        float x = Mathf.Cos(angle) * xAxisRadius;
        float y = Mathf.Sin(angle) * yAxisRadius;

        spotLight.transform.localPosition = new Vector3(x, 0, y);

        // detection spehere middle point
        gizmoSphereCenter = spotLight.transform.position + spotLight.transform.forward * detectionSphereDistance;
        gizmoSphereRadius = detectionSphereRadius;

        detectedColliders = Physics.OverlapSphere(gizmoSphereCenter, gizmoSphereRadius, hitLayers);

        if (detectedColliders.Length > 0)
        {
            gizmoSphereDetected = true;
            foreach (Collider col in detectedColliders)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    Vector3 tempPos = spawnPointObj.transform.position;
                    tempPos.z += i;
                    Instantiate(enemyPrefab, tempPos, spawnPointObj.transform.rotation);
                }
                spawnCount = 0;
                //Debug.Log($"Detection collided with: {col.name}");
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

        // ellipse - yellow
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
        Gizmos.DrawLine(previousPoint, transform.position + new Vector3(xAxisRadius, 0, 0)); // close ellipse

        // detection sphere
        if (gizmoSphereDetected)
        {
            // detected -> green
            Gizmos.color = Color.green;
        }
        else
        {
            // didnt detect -> red
            Gizmos.color = Color.red;
        }

        if (Application.isPlaying && gizmoSphereDetected)
        {
            Gizmos.DrawSphere(gizmoSphereCenter, gizmoSphereRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(gizmoSphereCenter, gizmoSphereRadius);
        }

        // draw detected obj. -> blue
        if (Application.isPlaying && gizmoSphereDetected && detectedColliders != null)
        {
            Gizmos.color = Color.blue;
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
