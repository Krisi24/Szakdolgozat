using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public float cooldownTime = 1.2f;
    private float damage = 10f;
    public GameObject EnemyTarget { get; set; }
    public Transform AttackBox;
    public Vector3 attackRange = new Vector3(2f, 1.75f, 1.25f);
    public Quaternion attackBoxRotation = new Quaternion();

    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine, Transform AttackBox) : base(enemy, enemyStateMachine)
    {
        this.AttackBox = AttackBox;
    }

    public override void EnterState()
    {
        //Debug.Log("Enter Attack State");
        base.EnterState();
        enemy.anim.SetBool("isAttack", true); // Elindítja a támadás animációt
        enemy.StartCoroutine(WaitForAttackToEnd()); // Várakoztatás, hogy befejezze
    }

    public override void ExitState()
    {
        //Debug.Log("Exit Attack State");
        base.ExitState();
        enemy.anim.SetBool("isAttack", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.RotateEnemyToPLayer();
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
    }

    private IEnumerator WaitForAttackToEnd()
    {
        Attack();
        yield return new WaitForSeconds(cooldownTime); // Megvárja a támadás végét
        enemyStateMachine.ChangeState(enemy.ChaseState); // Visszavált üldözés módba
    }

    void Attack()
    {
        //Debug.Log("Attack happend");
        Collider[] hitEnemies = Physics.OverlapBox(AttackBox.position, attackRange, attackBoxRotation);

        foreach (Collider enemy in hitEnemies)
        {
            //Debug.Log("hit enemy by AI: " + enemy.name);
            if (!enemy.isTrigger)
            {
                Player enemyComponent = enemy.GetComponent<Player>();  // Ellenõrizzük, hogy van-e PLayer
                if (enemyComponent != null)
                {
                    enemyComponent.Damage(damage);
                }
            }
        }
    }
}
