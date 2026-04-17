using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(SlimeEnemyBehavior behavior) : base(behavior) { }

    public override void Enter()
    {
        Debug.Log($"[{_enemy.name}] Entering Patrol state");

        _chaser.StopChase();
        _attacker.ClearTarget();
        _patrolMover.Patrol();
    }

    public override void Exit()
    {
        _patrolMover.StopPatrol();
    }
}
