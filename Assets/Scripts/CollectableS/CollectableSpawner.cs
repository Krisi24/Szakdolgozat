using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
[System.Serializable]
public class GroundColliderRow: List<BoxCollider>
{
    public List<BoxCollider> rowColliders = new List<BoxCollider>();
}*/

/*
 Boxcollider csoportonként egy helyen spawnol egy Objektumot
 */

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objPrefab;
    [SerializeField] private List<GameObject> groundCollidersHolder;
    private List<List<BoxCollider>> groundCollidersList = new List<List<BoxCollider>>();
    [SerializeField] private int amountToSpawn = 5;

    void Start()
    {
        if(groundCollidersHolder.Count == 0)
        {
            Debug.LogWarning("Obj groundCollidersHolder is not assigned in the Inspector!");
            return;
        }

        // get all boxColliders
        groundCollidersHolder.ForEach(x => {
                groundCollidersList.Add(
                    new List<BoxCollider>(
                        x.GetComponentsInChildren<BoxCollider>()
                    )
                );
            });

        SpawnCollectables();
    }

    void SpawnCollectables()
    {
        // check prerequisites
        if (objPrefab == null)
        {
            Debug.LogError("Obj Prefab is not assigned in the Inspector!", this);
            return;
        }

        if(groundCollidersList.Count == 0)
        {
            Debug.LogWarning("No ground colliders List assigned to spawn on!", this);
            return;
        }

        foreach (List<BoxCollider> colliders in groundCollidersList)
        {
            if(colliders.Count == 0)
            {
                Debug.LogWarning("No ground colliders assigned to spawn on!", this);
                return;
            }
        }

        // spawn every Game Object
        int listsAmount = groundCollidersList.Count;

        for (int i = amountToSpawn; i > 0; i--)
        {
            List<BoxCollider> colliderList = groundCollidersList[Random.Range(0, listsAmount)];
            
            int colliderAmount = colliderList.Count;
            BoxCollider currentCollider = colliderList[Random.Range(0, colliderAmount)];
            Vector3 spawnPos = RandomPointInBounds(currentCollider.bounds);

            /*
            Renderer prefabRenderer = objPrefab.GetComponent<Renderer>();
            if (prefabRenderer != null)
            {
                spawnPos.y = currentCollider.bounds.min.y + prefabRenderer.bounds.extents.y;
            }
            else
            {
                spawnPos.y = currentCollider.bounds.min.y + 1f;
            } */

            Instantiate(objPrefab, spawnPos, Quaternion.identity, transform);
        }
    }


    // returns a random point in the bounds
    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
