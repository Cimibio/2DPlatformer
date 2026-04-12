using System.Collections;
using UnityEngine;

public class SlimeEnemyBehavior : EnemyBehavior
{
    [Header("Slime Settings")]
    [SerializeField] private float _forgetDelay = 5f;
    [SerializeField] private bool _debugMode = true;

    private bool _isReturningToLastPosition = false;
    private float _forgetTimer;
    private Coroutine _forgetCoroutine;

    public override void OnEnable()
    {
        base.OnEnable();

        TargetDetector.TargetDetected += OnTargetDetected;
        TargetDetector.TargetPositionUpdated += OnTargetPositionUpdated;
        TargetDetector.LineOfSightLost += OnLineOfSightLost;
        TargetDetector.TargetLost += OnTargetLost;

        Chaser.ChaseStarted += OnChaseStarted;
        Chaser.ChaseStopped += OnChaseStopped;
        Chaser.TargetReached += OnTargetReached;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        TargetDetector.TargetDetected -= OnTargetDetected;
        TargetDetector.TargetPositionUpdated -= OnTargetPositionUpdated;
        TargetDetector.LineOfSightLost -= OnLineOfSightLost;
        TargetDetector.TargetLost -= OnTargetLost;

        Chaser.ChaseStarted -= OnChaseStarted;
        Chaser.ChaseStopped -= OnChaseStopped;
        Chaser.TargetReached -= OnTargetReached;

        StopForgetTimer();
    }

    public override void Update()
    {
        base.Update();

        if (TargetDetector.HasTarget && TargetDetector.HasLineOfSight)
        {
            ResetForgetTimer();

            if (!Chaser.IsChasing)
            {
                PatrolMover.StopPatrol();
                Chaser.StartChase(TargetDetector.Target.position);
            }
        }
        else if (TargetDetector.HasTarget && !TargetDetector.HasLineOfSight)
        {
            if (!_isReturningToLastPosition && !Chaser.IsChasing)
            {
                _isReturningToLastPosition = true;
                Chaser.StartChase(TargetDetector.LastKnownPosition);
            }
        }
        else if (!TargetDetector.HasTarget && !Chaser.IsChasing && !PatrolMover.IsPatrolling)
        {
            PatrolMover.StartPatrol();
        }
    }

    public override void Init()
    {
        base.Init();

        if (!TargetDetector.HasTarget)
        {
            PatrolMover.StartPatrol();
        }
    }

    private void OnTargetDetected(Transform target)
    {
        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Target detected! Starting chase.");

        StopForgetTimer();
        PatrolMover.StopPatrol();
        Chaser.StartChase(target.position);
    }

    private void OnTargetPositionUpdated(Vector3 position)
    {
        if (Chaser.IsChasing)
        {
            Chaser.UpdateTarget(position);
            ResetForgetTimer();
        }
    }

    private void OnLineOfSightLost()
    {
        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Line of sight lost. Moving to last known position.");

        _isReturningToLastPosition = true;
        Chaser.UpdateTarget(TargetDetector.LastKnownPosition);
        StartForgetTimer();
    }

    private void OnTargetLost()
    {
        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Target left vision zone.");

        _isReturningToLastPosition = false;

        if (Chaser.IsChasing)
        {
            StartForgetTimer();
        }
    }

    private void OnChaseStarted()
    {
        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Chase started.");
    }

    private void OnChaseStopped()
    {
        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Chase stopped.");

        StopForgetTimer();
        _isReturningToLastPosition = false;

        if (_enemy.IsAlive && !TargetDetector.HasTarget)
        {
            PatrolMover.StartPatrol();
        }
    }

    private void OnTargetReached()
    {
        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Target position reached.");

        if (_isReturningToLastPosition)
        {
            StartForgetTimer();
        }
    }

    private void StartForgetTimer()
    {
        StopForgetTimer();
        _forgetCoroutine = StartCoroutine(ForgetTimerCoroutine());
    }

    private void StopForgetTimer()
    {
        if (_forgetCoroutine != null)
        {
            StopCoroutine(_forgetCoroutine);
            _forgetCoroutine = null;
        }
        _forgetTimer = 0;
    }

    private void ResetForgetTimer()
    {
        if (_forgetCoroutine != null)
        {
            StopCoroutine(_forgetCoroutine);
            _forgetCoroutine = StartCoroutine(ForgetTimerCoroutine());
        }
        _forgetTimer = 0;
    }

    private IEnumerator ForgetTimerCoroutine()
    {
        _forgetTimer = 0;

        while (_forgetTimer < _forgetDelay)
        {
            _forgetTimer += Time.deltaTime;
            yield return null;
        }

        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Forgot about target after {_forgetDelay} seconds.");

        ForgetTarget();
    }

    private void ForgetTarget()
    {
        StopForgetTimer();
        _isReturningToLastPosition = false;
        Chaser.StopChase();

        if (_enemy.IsAlive)
        {
            PatrolMover.StartPatrol();
        }
    }
}