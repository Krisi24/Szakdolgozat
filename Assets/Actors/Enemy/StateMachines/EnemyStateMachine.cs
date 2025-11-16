public class EnemyStateMachine
{
    public EnemyState CurrentEnemyState { get; set; }

    public void Initalize (EnemyState enemyStartingState)
    {
        CurrentEnemyState = enemyStartingState;
        CurrentEnemyState.EnterState();
    }

    public void ChangeState(EnemyState newState)
    {
        //Debug.Log(newState.ToString());
        CurrentEnemyState.ExitState();
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();
    }
}
