using UnityEngine;

public class RollState : PlayerState
{
    private float nextRollTime = 0f;
    private float coolDownTime = 1f;

    public RollState(Player player) : base(player)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.ChangeAnimation("Roll");
        nextRollTime = coolDownTime + Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();
        player.ResetMaxLinearVelocity();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
        if (nextRollTime > Time.time)
        {
            player.Move(player.transform.forward);
        } else
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
}
