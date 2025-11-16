using UnityEngine;
using UnityEngine.AI;

public class WaterBorder : MonoBehaviour
{
    [SerializeField] private float searchRadius = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            FindAndMovePlayerToShore(other.gameObject);
        }
    }

    private void FindAndMovePlayerToShore(GameObject player)
    {
        Vector3 searchStartPosition = GetComponent<Collider>().ClosestPoint(player.transform.position);

        if (NavMesh.SamplePosition(searchStartPosition, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
        {
            player.transform.position = hit.position;
        }
        else
        {
            Debug.LogWarning("There is no walkable NavMeshSurface");
        }
    }
}
