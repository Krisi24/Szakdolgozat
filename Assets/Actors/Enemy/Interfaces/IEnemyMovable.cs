using UnityEngine;

public interface IEnemyMovable
{
    Rigidbody RB { get; set; }
    bool isFacingRight { get; set; }


    bool MoveEnemyToPlayer();
}
