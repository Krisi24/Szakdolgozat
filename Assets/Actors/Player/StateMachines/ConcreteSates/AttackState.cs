using UnityEngine;
using System.Collections;

public class AttackState : PlayerState
{
    private float cooldownTime = 1.2f;
    private float nextfireTime = 0f;
    private float damage = 40f;
    public GameObject EnemyTarget { get; set; }

    public Transform AttackBox;
    public Vector3 attackRange = new Vector3(2f, 1.75f, 1.25f);
    public Quaternion attackBoxRotation = new Quaternion();


    public AttackState(Player player, PlayerStateMachine playerStateMachine) : base(player)
    {
        this.AttackBox = AttackBox;
    }

    public override void EnterState()
    {
        base.EnterState();
        if (IsUsable())
        {
            player.StartCoroutine(WaitForAttackToEnd());
        }
    }

    public bool IsUsable()
    {
        return nextfireTime < Time.time;
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


    private IEnumerator WaitForAttackToEnd()
    {
        nextfireTime = Time.time + cooldownTime;
        Attack();
        yield return new WaitForSeconds(0.6f); // Megvárja a támadás végét
    }

    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapBox(AttackBox.position, attackRange, attackBoxRotation);

        foreach (Collider enemy in hitEnemies)
        {
            //Debug.Log("hit enemy: " + enemy.name);
            if (!enemy.isTrigger)
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();  // Ellenõrizzük, hogy van-e Enemy komponens
                if (enemyComponent != null)
                {
                    enemyComponent.Damage(damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackBox == null)
        {
            return;
        }
        Gizmos.DrawWireCube(AttackBox.position, attackRange);
    }


}
