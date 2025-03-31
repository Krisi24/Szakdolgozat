using UnityEngine;

public class MoveState : PlayerState
{

    public MoveState(Player player, PlayerStateMachine playerStateMachine, Animator playerAnim) : base(player, playerStateMachine, playerAnim)
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
        if (player.GetMoveInput() != Vector3.zero)
        {
            player.Move(player.GetMoveInput(), 10);
        }
    }
}
