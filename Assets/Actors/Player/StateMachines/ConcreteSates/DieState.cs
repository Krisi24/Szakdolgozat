using UnityEngine;

public class DieState : PlayerState
{


    public DieState(Player player) : base(player)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.Die();
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
}
