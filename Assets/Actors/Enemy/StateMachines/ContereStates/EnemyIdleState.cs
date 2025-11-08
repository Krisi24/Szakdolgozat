using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private float maxIdleTime = 3f;
    private float idleStartTime;

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.ChangeAnimation("Idle");
        idleStartTime = Time.realtimeSinceStartup;
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
            else { 
                enemy.StateMachine.ChangeState(enemy.SearchState);
            }
        } 
        if ((idleStartTime + maxIdleTime) <= Time.realtimeSinceStartup)
        {
            enemy.newPatrolPoint();
            enemy.StateMachine.ChangeState(enemy.PatrolState);
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
    }
}
