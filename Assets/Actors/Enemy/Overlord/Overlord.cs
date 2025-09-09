using NUnit.Framework.Internal.Filters;
using UnityEngine;

public class Overlord : MonoBehaviour
{

    [SerializeField] private Enemy[] team1;
    [SerializeField] private float notifyDistance = 100f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NotifyOthers(Vector3 seenPos, Enemy notifier)
    {
        foreach(Enemy enemy in team1){
            if(enemy == notifier || enemy == null 
                || enemy.StateMachine.CurrentEnemyState == enemy.AttackState
                || enemy.StateMachine.CurrentEnemyState == enemy.DieState
                || enemy.StateMachine.CurrentEnemyState == enemy.ChaseState)
            {
                continue;
            }
            float distance = Vector3.Distance(enemy.transform.position, seenPos);
            if(distance < notifyDistance)
            {
                //enemy.NotifyDetection(seenPos);
            }
        }
    }
}
