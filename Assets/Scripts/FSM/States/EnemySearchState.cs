using UnityEngine;

public class EnemySearchState : EnemySubState
{
    private Vector3 _lastKnownPosition;
    private float _stuckTimer;
    private float _lastXPosition;
    private bool _isComplete;

    public EnemySearchState(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
        : base(behavior, combatState) { }

    public bool IsComplete => _isComplete;

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] → Search (target: {_targeter.LastKnownPosition})");

        _isComplete = false;
        _stuckTimer = 0f;

        _lastKnownPosition = _targeter.LastKnownPosition;
        _lastXPosition = _enemy.transform.position.x;

        _chaser.StopChase();
        _patrolMover.StopPatrol();
        _attacker.ClearTarget();

        _searcher.Search(_lastKnownPosition);
    }

    public override void Update()
    {
        if (_isComplete)
            return;

        if (_searcher.IsReached)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Reached last known position");

            _isComplete = true;
            return;
        }

        ProcessStuckCheck();
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
        _searcher.StopSearch();
        _isComplete = false;
    }
}