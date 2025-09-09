using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyChaseState : EnemyState
{
    public static event Action<Vector3, Vector3> OnNotifyAboutPlayer;

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.XrayOn();
        enemy.ChangeAnimation("Run");
        enemy.IsPlayerSeen = true;
        //enemy.overlord.NotifyOthers(enemy.playerLastPosition, enemy);
        OnNotifyAboutPlayer?.Invoke(enemy.playerLastPosition, enemy.transform.position);
    }
    public override void ExitState()
    {
        base.ExitState();
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
