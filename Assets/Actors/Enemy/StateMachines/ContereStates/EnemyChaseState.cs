using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("isMoving", true);

        enemy.IsPlayerSeen = true;

        enemy.overlord.NotifyOthers(enemy.playerLastPosition, enemy);
    }
    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("isMoving", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!enemy.isAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
        if (enemy.MoveEnemyToPlayer())
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }
}
