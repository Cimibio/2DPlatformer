using UnityEngine;

public class EnemySearchState : EnemyState
{
    private bool _hasReachedPosition;

    public bool HasReachedPosition => _hasReachedPosition;

    public EnemySearchState(SlimeEnemyBehavior behavior) : base(behavior) { }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Entering Search state - moving to last known position");

        _hasReachedPosition = false;

        Vector3 lastKnownPosition = _targeter.LastKnownPosition;
        _chaser.Chase(lastKnownPosition);
        _attacker.ClearTarget();

        _patrolMover.StopPatrol();
    }

    public override void Update()
    {
        if (_hasReachedPosition)
            return;

        if (!_chaser.IsChasing)
        {
            _hasReachedPosition = true;

            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Reached last known position");

            //_behavior.OnReachedLastKnownPosition();
        }
    }

    public override void Exit()
    {
        _chaser.StopChase();
        _hasReachedPosition = false;

        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Exiting Search state");
    }
}