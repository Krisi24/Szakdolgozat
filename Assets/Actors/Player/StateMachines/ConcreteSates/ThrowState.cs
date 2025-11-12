using UnityEngine;

public class ThrowState : PlayerState
{
    private float cooldownTime = 2.5f;
    private float exitStateTime = 0f;
    private float spawnThroableTime = Mathf.Infinity;
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
        exitStateTime = Time.time + cooldownTime;
        spawnThroableTime = Time.time + ((2f + 23f/60) / 2);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if(spawnThroableTime < Time.time)
        {
            player.SpawnThroable();
            spawnThroableTime = Mathf.Infinity;
        }
        if(exitStateTime < Time.time)
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
    }
}
