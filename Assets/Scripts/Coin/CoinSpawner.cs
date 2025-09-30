using Unity.VisualScripting;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private BoxCollider groundCollider;
    [SerializeField] private int coinAmount = 10;

    void Start()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            Vector3 startPos = RandomPointsInGround(groundCollider.bounds);
            Instantiate(coinPrefab, startPos, Quaternion.identity);
        }
        
    }

    public static Vector3 RandomPointsInGround(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            1f, 
            Random.Range(bounds.min.z, bounds.max.z)
        );   
    }

    void Update()
    {
        
    }
}
