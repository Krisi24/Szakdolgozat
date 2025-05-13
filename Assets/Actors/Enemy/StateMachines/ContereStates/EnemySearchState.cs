using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemySearchState : EnemyState
{
    public EnemySearchState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("isMoving", true);
    }
    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("isMoving", false);
        enemy.IsPlayerSeen = false;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.isAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
        if (enemy.MoveEnemyToPosSmart(enemy.notifyPos))
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }
    }
}
