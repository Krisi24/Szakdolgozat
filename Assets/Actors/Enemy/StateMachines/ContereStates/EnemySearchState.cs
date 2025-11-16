public class EnemySearchState : EnemyState
{
    public EnemySearchState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.SetRoute();
        enemy.ChangeAnimation("Run");
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

        //Debug.Log("agro: " + enemy.isAggroed + "avaiable: " + enemy.PlayerIsDirectlyAvailable());
        if (enemy.isAggroed && enemy.PlayerIsDirectlyAvailable())
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }
        if (enemy.MoveEnemyToPosSmart(enemy.GetNextMovePosition()))
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }
    }
}
