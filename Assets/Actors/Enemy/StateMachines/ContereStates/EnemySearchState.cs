using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemySearchState : EnemyState
{
    public EnemySearchState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
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
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
        if (enemy.MoveEnemyToLastSeenPos())
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }
    }
}
