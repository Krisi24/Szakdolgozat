using UnityEngine;

public class ThrowState : PlayerState
{
    private float cooldownTime = 2.5f;
    private float nextfireTime = 0f;
    public GameObject EnemyTarget { get; set; }

    public Transform AttackBox;
    public Vector3 attackRange = new Vector3(2f, 1.75f, 1.25f);
    public Quaternion attackBoxRotation = new Quaternion();


    public ThrowState(Player player, Transform AttackBox) : base(player)
    {
        this.AttackBox = AttackBox;
    }

    public override void EnterState()
    {
        base.EnterState();
        player.ChangeAnimation("Throw");
        nextfireTime = Time.time + cooldownTime;
        Throw();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if(nextfireTime < Time.time)
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
    }

    private void Throw()
    {
        Debug.Log("throw");
    }
}
