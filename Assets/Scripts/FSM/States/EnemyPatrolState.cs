using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(SlimeEnemyBehavior behavior) : base(behavior) { }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Entering Patrol state");

        _patrolMover.Patrol();
    }

    //public override void Update() { }

    public override void Exit()
    {
        _patrolMover.StopPatrol();
    }
}