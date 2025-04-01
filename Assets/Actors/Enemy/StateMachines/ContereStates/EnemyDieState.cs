using System.Collections;
using UnityEngine;

public class EnemyDieState : EnemyState
{
    public EnemyDieState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("isMoving", false);
        enemy.anim.SetBool("isAttack", false);
        enemy.anim.SetBool("isDead", true);
        enemy.StartCoroutine(WaitForDie());
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
    }

    private IEnumerator WaitForDie()
    {
        yield return new WaitForSeconds(4f);
        enemy.Die();
    }
}
