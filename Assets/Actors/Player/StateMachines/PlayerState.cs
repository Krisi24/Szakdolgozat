using UnityEngine;

public class PlayerState
{

    protected Player player;
    protected PlayerStateMachine playerStateMachine;
    protected Animator playerAnim;

    public PlayerState(Player player, PlayerStateMachine playerStateMachine, Animator playerAnim)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
        this.playerAnim = playerAnim;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhisicsUpdate() { }
    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) { }
}
