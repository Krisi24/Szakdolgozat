using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentPlayerState { get; set; }

    public void Initalize(PlayerState playerStartingState)
    {
        CurrentPlayerState = playerStartingState;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }
}
