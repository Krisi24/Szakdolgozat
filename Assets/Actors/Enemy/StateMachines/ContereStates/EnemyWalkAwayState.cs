using UnityEngine;

public class EnemyWalkAwayState : EnemyState
{
    private Vector3 offsetHeight = new Vector3(0f, 0.5f, 0f);
    private Vector3 offsetRight;
    private Vector3 offsetLeft;
    private LayerMask obstructionLayerMask = LayerMask.GetMask("obstruction");
    private float avoidDistance = 1.5f;
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
        //AvoidObstaclesVersion1();
        AvoidObstaclesVersion2();
    }

    /*
    private void AvoidObstaclesVersion1()
    {
        if (!Physics.Raycast(enemy.transform.position + offsetHeight + offsetRight, enemy.transform.forward, avoidDistance, obstructionLayerMask) &&
            !Physics.Raycast(enemy.transform.position + offsetHeight + offsetLeft, enemy.transform.forward, avoidDistance, obstructionLayerMask))
        {
            enemy.MoveEnemyToPos(enemy.transform.position + enemy.transform.forward);
        }
        else if (!Physics.Raycast(enemy.transform.position + offsetHeight + offsetForward, -enemy.transform.right, avoidDistance, obstructionLayerMask) &&
                  !Physics.Raycast(enemy.transform.position + offsetHeight + offsetBack, -enemy.transform.right, avoidDistance, obstructionLayerMask))
        {
            enemy.MoveEnemyToPos(enemy.transform.position - enemy.transform.right);
        }
        else if (!Physics.Raycast(enemy.transform.position + offsetHeight + offsetForward, enemy.transform.right, avoidDistance, obstructionLayerMask) &&
                 !Physics.Raycast(enemy.transform.position + offsetHeight + offsetBack, enemy.transform.right, avoidDistance, obstructionLayerMask))
        {
            enemy.MoveEnemyToPos(enemy.transform.position + enemy.transform.right);
        }
        else
        {
            enemy.MoveEnemyToPos(enemy.transform.position - enemy.transform.forward);
        }
    }
    */

    private void AvoidObstaclesVersion2()
    {
        offsetRight = enemy.transform.right * 0.25f;
        offsetLeft = -enemy.transform.right * 0.25f;

        //forward
        Debug.DrawRay(enemy.transform.position + offsetHeight + offsetRight, enemy.transform.forward * avoidDistance, Color.yellow);
        Debug.DrawRay(enemy.transform.position + offsetHeight + offsetLeft, enemy.transform.forward * avoidDistance, Color.yellow);
        Debug.DrawRay(enemy.transform.position + offsetHeight + offsetRight, (enemy.transform.forward + offsetRight * 2).normalized * avoidDistance, Color.yellow);
        Debug.DrawRay(enemy.transform.position + offsetHeight + offsetLeft, (enemy.transform.forward + offsetLeft * 2).normalized * avoidDistance, Color.yellow);

        // left & right
        Debug.DrawRay(enemy.transform.position + offsetHeight, enemy.transform.right * avoidDistance, Color.yellow);
        Debug.DrawRay(enemy.transform.position + offsetHeight, -enemy.transform.right * avoidDistance, Color.yellow);

        if (
            Physics.Raycast(enemy.transform.position + offsetHeight + offsetRight, enemy.transform.forward, avoidDistance, obstructionLayerMask) ||
            Physics.Raycast(enemy.transform.position + offsetHeight + offsetLeft, enemy.transform.forward, avoidDistance, obstructionLayerMask) ||
            Physics.Raycast(enemy.transform.position + offsetHeight + offsetRight, (enemy.transform.forward + offsetRight * 2).normalized, avoidDistance, obstructionLayerMask) ||
            Physics.Raycast(enemy.transform.position + offsetHeight + offsetLeft, (enemy.transform.forward + offsetLeft * 2).normalized, avoidDistance, obstructionLayerMask)
            )
        {
            bool isRightBlocked = Physics.Raycast(enemy.transform.position + offsetHeight, enemy.transform.right, avoidDistance, obstructionLayerMask);
            bool isLeftBlocked = Physics.Raycast(enemy.transform.position + offsetHeight, -enemy.transform.right, avoidDistance, obstructionLayerMask);

            enemy.TurnLeftOrRight(isRightBlocked && !isLeftBlocked);
        } else
        {
            enemy.MoveEnemyToPos(enemy.transform.position + enemy.transform.forward);
        }
    }
}
