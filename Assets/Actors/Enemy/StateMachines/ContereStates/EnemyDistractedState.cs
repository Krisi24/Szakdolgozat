using UnityEngine;

public class EnemyDistractedState : EnemyState
{
    private GameObject distractionObject;
    private bool hasArrived;
    private float recalculatePathTime = 0;

    public EnemyDistractedState(Enemy enemy, EnemyStateMachine enemyStateMachine, GameObject distractionObject) : base(enemy, enemyStateMachine)
    {
        this.distractionObject = distractionObject;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.XrayOn();
        hasArrived = false;
        enemy.SetRouteToObject(distractionObject.transform.position);
        recalculatePathTime += Time.time + 1f;
        enemy.ChangeAnimation("Run");
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!hasArrived)
        {
            if (recalculatePathTime < Time.time)
            {
                recalculatePathTime += Time.time + 1f;
                enemy.SetRouteToObject(distractionObject.transform.position);
            }
            float distance = Vector3.Distance(enemy.transform.position, distractionObject.transform.position);
            if (distance < 0.5f)
            {
                hasArrived = true;
                enemy.ChangeAnimation("Distracted");
            }
            else
            {
                enemy.MoveEnemyToPosSmart(enemy.GetNextMovePosition());
            }
        }
    }

    public override void PhisicsUpdate()
    {
        base.PhisicsUpdate();
    }
}
