using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private readonly float _idleDuration;
    private float _timer;
    private bool _isWaiting;

    public bool IsIdleComplete => !_isWaiting;

    public EnemyIdleState(SlimeEnemyBehavior behavior) : base(behavior)
    {
        _idleDuration = behavior.ForgetDelay;
    }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Entering Idle state - searching for target ({_idleDuration}s)");

        _isWaiting = true;
        _timer = 0f;

        _chaser.StopChase();
        _patrolMover.StopPatrol();
        _attacker.ClearTarget();
    }

    public override void Update()
    {
        if (!_isWaiting)
            return;

        if (_targeter.HasTarget && _targeter.HasLineOfSight)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Target spotted during idle!");

            _isWaiting = false;

            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= _idleDuration)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Idle timer complete - target forgotten");

            _isWaiting = false;
            //_behavior.OnIdleComplete();
        }
    }

    public override void Exit()
    {
        _isWaiting = false;
        _timer = 0f;

        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Exiting Idle state");
    }
}