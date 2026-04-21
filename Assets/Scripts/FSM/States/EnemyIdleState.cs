using UnityEngine;

public class EnemyIdleState : EnemySubState
{
    private readonly float _idleDuration;
    private float _timer;
    private bool _isComplete;

    public EnemyIdleState(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
        : base(behavior, combatState)
    {
        _idleDuration = behavior.ForgetDelay;
    }

    public bool IsComplete => _isComplete;

    public override void Update()
    {
        if (_isComplete)
            return;

        _timer += Time.deltaTime;

        if (_timer >= _idleDuration)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Idle complete - target forgotten, returning to Patrol");

            _isComplete = true;
        }
    }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] → Idle ({_idleDuration}s)");

        _isComplete = false;
        _timer = 0f;

        _searcher.StopSearch();
        _chaser.StopChase();
        _patrolMover.StopPatrol();
        _attacker.ClearTarget();
    }

    public override void Exit()
    {
        _timer = 0f;
        _isComplete = false;
    }
}