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
        base.EnterState();
        enemy.ChangeAnimation("Attack");
        enemy.StartCoroutine(WaitForAttackToEnd());
    }

    public override void ExitState()
    {
        //Debug.Log("Exit Attack State");
        base.ExitState();
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
        yield return new WaitForSeconds(cooldownTime);
        if ((enemy.StateMachine.CurrentEnemyState != enemy.DieState) &&
            (enemy.StateMachine.CurrentEnemyState != enemy.WalkAwayState) &&
            (enemy.StateMachine.CurrentEnemyState != enemy.DistractedState)
            )
        {
            enemy.StateMachine.ChangeState(enemy.SearchState);
        }
    }

    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapBox(AttackBox.position, attackRange, attackBoxRotation);

        foreach (Collider enemy in hitEnemies)
        {
            if (!enemy.isTrigger)
            {
                Player enemyComponent = enemy.GetComponent<Player>();
                if (enemyComponent != null)
                {
                    enemyComponent.Damage(damage);
                }
            }
        }
    }
}
