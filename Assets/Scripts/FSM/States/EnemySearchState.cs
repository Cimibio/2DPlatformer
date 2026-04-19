using UnityEngine;

public class EnemySearchState : EnemySubState
{
    private Vector3 _lastKnownPosition;
    private float _stuckTimer;
    private float _lastXPosition;
    private bool _isComplete;

    public bool IsComplete => _isComplete;

    public EnemySearchState(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
        : base(behavior, combatState) { }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] → Search");

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

        float currentX = _enemy.transform.position.x;
        if (Mathf.Abs(currentX - _lastXPosition) < _behavior.StuckThreshold)
        {
            _stuckTimer += Time.deltaTime;

            if (_stuckTimer >= _behavior.StuckTime)
            {
                if (_behavior.DebugMode)
                    Debug.Log($"[{_enemy.name}] Stuck during search!");

                _isComplete = true;
                _combatState.OnSearchStuck();
                return;
            }
        }
        else
        {
            _stuckTimer = 0f;
            _lastXPosition = currentX;
        }

        if (!_chaser.IsChasing)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Reached last known position");

            _isComplete = true;
            _combatState.OnReachedLastKnownPosition();
        }
    }

    public override void Exit()
    {
        _chaser.StopChase();
        _isComplete = false;
    }
}