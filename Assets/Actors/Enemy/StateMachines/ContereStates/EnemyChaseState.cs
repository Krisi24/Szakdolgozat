using System;
using UnityEditor;
using UnityEngine;

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
        OnNotifyAboutPlayer?.Invoke(enemy.playerLastPosition, enemy.transform.position);
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
        if (!enemy.isAggroed || !enemy.PlayerIsDirectlyAvailable())
        {
            enemy.StateMachine.ChangeState(enemy.SearchState);
            return;
        }
        if (enemy.MoveEnemyToPlayer())
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }
}
