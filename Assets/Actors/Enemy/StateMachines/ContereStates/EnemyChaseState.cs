using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public static event Action<Vector3, Vector3> OnNotifyAboutPlayer;
    private EnemyVisionCheck vision;

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        vision = enemy.GetComponent<EnemyVisionCheck>();
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
        if (!vision.TargetIsVisible())
        {
            enemy.StateMachine.ChangeState(enemy.SearchState);
            return;
        }
        if (enemy.MoveEnemyToPlayer())
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
    }
}
