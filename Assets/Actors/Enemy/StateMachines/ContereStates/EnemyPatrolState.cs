using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyPatrolState : EnemyState
{

    private Vector3 patrolEndpoint;
    private Vector3 patrolStartpoint;
    private bool isMovingBack = false;
    public EnemyPatrolState(Enemy enemy, EnemyStateMachine enemyStateMachine, Vector3 patrolEndpoint) : base(enemy, enemyStateMachine)
    {
        patrolStartpoint = enemy.transform.position;
        this.patrolEndpoint = patrolEndpoint;
    }

    public override void EnterState()
    {
        base.EnterState();
        patrolStartpoint = enemy.transform.position;
        this.patrolEndpoint = enemy.GetPatrolEndpoint();
        enemy.ChangeAnimation("Walking");
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.isAggroed)
        {
            if (enemy.PlayerIsDirectlyAvailable())
            {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
            }
            else
            {
                enemy.StateMachine.ChangeState(enemy.SearchState);
            }
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
