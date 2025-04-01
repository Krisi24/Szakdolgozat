using UnityEngine;

public class MoveState : PlayerState
{

    public MoveState(Player player) : base(player)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.InputLvlZero();
        if (player.IsCrouches())
        {
            player.ChangeAnimation("Crouched Walking");
        } else
        {
            player.ChangeAnimation("Run");
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
        if (player.GetMoveInput() != Vector3.zero)
        {
            if (player.IsCrouches())
            {
                player.Move(player.GetMoveInput());
            }
            else
            {
                player.Move(player.GetMoveInput());
            }
        } else
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
}
