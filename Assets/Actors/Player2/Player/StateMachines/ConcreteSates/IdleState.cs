using UnityEngine;

public class IdleState : PlayerState
{

    public IdleState(PlayerHandler player, PlayerStateMachine playerStateMachine, Animator playerAnim) : base(player, playerStateMachine, playerAnim)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        
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
        player.PlayerMove();
    }
}
