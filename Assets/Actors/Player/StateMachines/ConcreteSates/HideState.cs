using UnityEngine;

public class HideState : PlayerState
{
    private Vector3 _targetPos;
    private Vector3 _direction;


    public HideState(Player player, PlayerStateMachine playerStateMachine, Animator playerAnim) : base(player, playerStateMachine, playerAnim)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        playerAnim.SetBool("isCrouch", true);
        player.SetSpeedToCrouch();
    }

    public override void ExitState()
    {
        base.ExitState();
        playerAnim.SetBool("isCrouch", false);
        player.SetSpeedToRun();
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
