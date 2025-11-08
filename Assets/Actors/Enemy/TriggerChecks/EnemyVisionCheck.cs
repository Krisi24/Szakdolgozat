using System.Collections;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyVisionCheck : MonoBehaviour
{

    [SerializeField] private float radius = 20f;
    [SerializeField] private float surroundingRadius = 1.5f;
    [SerializeField] private float angle;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask obstructionLayerMask;
    private Enemy enemy;
    private GameObject player;
    private Vector3  hight = new Vector3(0f, 1.5f,0f);

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    public void StartSearch()
    {
        StartCoroutine("FOVRoutine");
    }

    public void EndSearch()
    {
        StopCoroutine("FOVRoutine");
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void SurroundingCheck()
    {
        Collider[] surroundingCheck = Physics.OverlapSphere(transform.position, surroundingRadius, playerLayerMask);
        if (surroundingCheck.Length != 0 && TargetIsReachable())
        {
            enemy.isAggroed = true;
            enemy.playerLastPosition = surroundingCheck[0].transform.position;
        } else
        {
            enemy.isAggroed = false;
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, playerLayerMask);

        if (rangeCheck.Length != 0)
        {
            Vector3 eyesPosition = transform.position + hight;
            Transform target = rangeCheck[0].transform;
            float distance = (target.position - transform.position).magnitude;
            Vector3 directionOfTarget = (target.position - transform.position).normalized;
            Vector3 directionOfTargetLow = ((target.position - hight/2)- transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionOfTarget) < angle / 2)
            {
                if (!((Physics.Raycast(eyesPosition, directionOfTarget, distance, obstructionLayerMask) ||
                    !Physics.Raycast(eyesPosition, directionOfTarget, distance, playerLayerMask)) &&
                    (Physics.Raycast(eyesPosition, directionOfTargetLow, distance, obstructionLayerMask) ||
                    !Physics.Raycast(eyesPosition, directionOfTargetLow, distance, playerLayerMask)))
                    )
                {
                    enemy.isAggroed = true;
                    enemy.playerLastPosition = target.position;
                    return;
                }
            }
        }
        SurroundingCheck();
    }

    public bool TargetIsReachable()
    {
        Vector3 legHight = new Vector3(0, 0.5f, 0);
        float distance = (player.transform.position - transform.position).magnitude;
        Vector3 directionOfTarget = (player.transform.position - transform.position).normalized;
        Debug.DrawLine(transform.position + legHight, player.transform.position + legHight, Color.gray, 1.5f);
        if(Physics.Raycast(transform.position + legHight, directionOfTarget, distance, playerLayerMask) &&
            !Physics.Raycast(transform.position + legHight, directionOfTarget, distance, obstructionLayerMask))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        // Surrounding Gizmos
        Gizmos.color = Color.white;   
        Gizmos.DrawWireSphere(transform.position, surroundingRadius);

        // Vision Gizmos
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, playerLayerMask);
        Vector3 eyesPosition = transform.position + hight;
        if (rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            float distance = (target.position - transform.position).magnitude;
            Vector3 directionOfTarget = (target.position - transform.position).normalized;
            Vector3 directionOfTargetLow = ((target.position - hight / 2) - transform.position).normalized;
            Vector3 forwardDirection = transform.forward;
            float angleToTarget = Vector3.Angle(forwardDirection, directionOfTarget);

            // elõre mutató egyenes
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, forwardDirection * radius);

            // körív
            Handles.color = Color.yellow;
            Vector3 fromDirection = Quaternion.AngleAxis(-angle / 2, transform.up) * forwardDirection;
            Handles.DrawWireArc(transform.position, transform.up, fromDirection, angle, radius);

            // látószög szélei
            Vector3 rightDirection = Quaternion.AngleAxis(angle / 2, transform.up) * forwardDirection;
            Gizmos.DrawRay(transform.position, fromDirection * radius);
            Gizmos.DrawRay(transform.position, rightDirection * radius);

            if (Vector3.Angle(transform.forward, directionOfTarget) < angle / 2)
            {
                Debug.DrawLine(eyesPosition, transform.position + hight + directionOfTarget * distance, Color.yellow, 1f);
                Debug.DrawLine(eyesPosition, transform.position + hight + directionOfTargetLow * distance, Color.yellow, 1f);
            }
        }
    }
}
