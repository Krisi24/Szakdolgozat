using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyPatrolState : EnemyState
{

    private Vector3 patrolEndpoint;
    private Vector3 patrolStartpoint;
    private bool isMovingBack = false;
    public EnemyPatrolState(Enemy enemy, EnemyStateMachine enemyStateMachine, Transform patrolEndpoint) : base(enemy, enemyStateMachine)
    {
        patrolStartpoint = enemy.transform.position;
        this.patrolEndpoint = patrolEndpoint.position;
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Enter Patrol State");
        enemy.anim.SetBool("isMoving", true);
    }
    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("isMoving", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.isAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
        else if (enemy.IsPlayerSeen)
        {
            enemy.StateMachine.ChangeState(enemy.SearchState);
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
        if(!isMovingBack)
        {
            if (enemy.MoveEnemyToPos(patrolEndpoint))
            {
                isMovingBack = true;
            }
        } else
        {
            if (enemy.MoveEnemyToPos(patrolStartpoint))
            {
                isMovingBack = false;
            }
        }
    }
}
