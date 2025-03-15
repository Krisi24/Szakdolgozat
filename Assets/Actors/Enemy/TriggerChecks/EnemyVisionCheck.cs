using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyVisionCheck : MonoBehaviour
{

    [SerializeField] private float radius = 20f;
    [SerializeField] private float angle;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstructionMask;
    private Enemy enemy;
    private Vector3  hight = new Vector3(0f, 1.5f,0f);

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
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

    private void FieldOfViewCheck()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, playerLayer);


        if (rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            float distance = (target.position - transform.position).magnitude;
            Vector3 directionOfTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionOfTarget) < angle / 2)
            {
                //Debug.DrawRay(transform.position + hight, directionOfTarget, new Color(255f, 0f, 0f), 1f);
                if (Physics.Raycast(transform.position + hight, directionOfTarget, distance, obstructionMask))
                {
                    enemy.isAggroed = false;
                } else
                {
                    enemy.isAggroed = true;
                    enemy.playerLastPosition = target.position;
                }
            }
            else
            {
                enemy.isAggroed = false;
            }
        }
        else if (enemy.isAggroed)
        {
            enemy.isAggroed = false;
        }
    }
}
