using UnityEngine;

public class IdleState : PlayerState
{

    public IdleState(Player player) : base(player)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.InputLvlZero();
        if (player.IsCrouches())
        {
            player.ChangeAnimation("Crouch Idle");
        }
        else
        {
            player.ChangeAnimation("Idle");
        }
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
