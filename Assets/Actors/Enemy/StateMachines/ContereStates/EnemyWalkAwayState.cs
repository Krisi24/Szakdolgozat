using UnityEngine;

public class EnemyWalkAwayState : EnemyState
{
    private Vector3 offset = new Vector3(0f, 0.5f, 0f);
    private LayerMask obstructionLayerMask = LayerMask.GetMask("obstruction");
    public EnemyWalkAwayState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.ChangeAnimation("Walking");
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {

    }

    public override void PhisicsUpdate()
    {
        if (Physics.Raycast(enemy.transform.position + offset, Vector3.forward, 2f, obstructionLayerMask))
        {
            enemy.MoveEnemyToPos(enemy.transform.position + Vector3.forward);
        } else if(Physics.Raycast(enemy.transform.position + offset, Vector3.left , 2f, obstructionLayerMask))
        {
            enemy.MoveEnemyToPos(enemy.transform.position + Vector3.left);
        }
        else if (Physics.Raycast(enemy.transform.position + offset, Vector3.right, 2f, obstructionLayerMask))
        {
            enemy.MoveEnemyToPos(enemy.transform.position + Vector3.right);
        }
        else if (Physics.Raycast(enemy.transform.position + offset, Vector3.back, 2f, obstructionLayerMask))
        {
            enemy.MoveEnemyToPos(enemy.transform.position + Vector3.back);
        }
    }
}
