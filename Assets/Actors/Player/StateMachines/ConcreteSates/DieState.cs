using UnityEngine;

public class DieState : PlayerState
{


    public DieState(Player player, PlayerStateMachine playerStateMachine, Animator playerAnim) : base(player, playerStateMachine, playerAnim)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
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
