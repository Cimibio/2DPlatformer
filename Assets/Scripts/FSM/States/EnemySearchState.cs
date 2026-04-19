using UnityEngine;

public class EnemySearchState : EnemySubState
{
    [SerializeField] private float _reachDistance = 0.2f;

    private Vector3 _lastKnownPosition;
    private float _stuckTimer;
    private float _lastXPosition;
    private bool _isComplete;
    private float _sqrReachDistance;

    public EnemySearchState(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
        : base(behavior, combatState)
    {
        _sqrReachDistance = _reachDistance * _reachDistance;
    }

    public bool IsComplete => _isComplete;

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] → Search (target: {_targeter.LastKnownPosition})");

        _isComplete = false;
        _stuckTimer = 0f;

        _lastKnownPosition = _targeter.LastKnownPosition;
        _lastXPosition = _enemy.transform.position.x;

        _chaser.Chase(_lastKnownPosition);
        _attacker.ClearTarget();
        _patrolMover.StopPatrol();
    }

    public override void Update()
    {
        if (_isComplete)
            return;

        if (HasReachedPosition())
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Reached last known position (distance: {Vector3.Distance(_enemy.transform.position, _lastKnownPosition):F2})");

            _isComplete = true;
            return;
        }

        ProcessStuckCheck();
    }

    private bool HasReachedPosition()
    {
        Vector2 currentPos = _enemy.transform.position;
        Vector2 targetPos = _lastKnownPosition;

        float sqrDistance = (currentPos - targetPos).sqrMagnitude;
        return sqrDistance <= _sqrReachDistance;
    }

    private void ProcessStuckCheck()
    {
        float currentXposition = _enemy.transform.position.x;

        if (Mathf.Abs(currentXposition - _lastXPosition) < _behavior.StuckThreshold)
        {
            _stuckTimer += Time.deltaTime;

            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Stuck during search! (stuck time: {_stuckTimer:F1}s)");

            if (_stuckTimer >= _behavior.StuckTime)
                _isComplete = true;
        }
        else
        {
            _stuckTimer = 0f;
            _lastXPosition = currentXposition;
        }
    }

    public override void Exit()
    {
        _chaser.StopChase();
        _isComplete = false;
    }
}