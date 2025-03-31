using UnityEngine;
using System.Collections;

public class RollState : PlayerState
{
    private float startTime;
    private float waitStartTime = 0.2f;
    private float nextRollTime = 0f;
    private float coolDownTime = 0.7f;

    public RollState(Player player, PlayerStateMachine playerStateMachine, Animator playerAnim) : base(player, playerStateMachine, playerAnim)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        playerAnim.SetBool("isMoving", false);
        playerAnim.SetBool("isCrouch", false);
        playerAnim.SetBool("isRoll", true);
        player.SetSpeedToRoll();
        startTime = Time.time + waitStartTime;
        nextRollTime = coolDownTime + Time.time;
        player.StartCoroutine(WaitForRollToEnd());
    }

    public override void ExitState()
    {
        base.ExitState();
        player.SetSpeedToRun();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
        if ( startTime < Time.time)
        {
            player.PlayerAddForce((player.transform.forward));
        }
    }

    private IEnumerator WaitForRollToEnd()
    {
        yield return new WaitForSeconds(0.6f); // Megvárja a támadás végét
        playerAnim.SetBool("isRoll", false);
        playerStateMachine.ChangeState(player.IdleState);
    }

    public bool IsUsable()
    {
        return nextRollTime < Time.time;
    }
}
