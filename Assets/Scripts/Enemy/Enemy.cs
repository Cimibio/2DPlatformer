using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMover), typeof(Chaser), typeof(TargetDetector))]
[RequireComponent(typeof(FallDetector))]
public class Enemy : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float _forgetDelay = 5f; // Время, после которого забываем об игроке
    [SerializeField] private bool _debugMode = true;

    private PatrolMover _patrolMover;
    private Chaser _chaser;
    private TargetDetector _targetDetector;
    private FallDetector _fallDetector;

    private bool _isAlive = true;
    private bool _isReturningToLastPosition = false;
    private float _forgetTimer;
    private Coroutine _forgetCoroutine;

    public event Action<Enemy> Falled;

    private void Awake()
    {
        _patrolMover = GetComponent<PatrolMover>();
        _chaser = GetComponent<Chaser>();
        _targetDetector = GetComponent<TargetDetector>();
        _fallDetector = GetComponent<FallDetector>();
    }

    private void OnEnable()
    {
        _targetDetector.TargetDetected += OnTargetDetected;
        _targetDetector.TargetPositionUpdated += OnTargetPositionUpdated;
        _targetDetector.LineOfSightLost += OnLineOfSightLost;
        _targetDetector.TargetLost += OnTargetLost;
        _chaser.ChaseStarted += OnChaseStarted;
        _chaser.ChaseStopped += OnChaseStopped;
        _chaser.TargetReached += OnTargetReached;
        _fallDetector.Falled += OnFalled;
    }

    private void OnDisable()
    {
        _targetDetector.TargetDetected -= OnTargetDetected;
        _targetDetector.TargetPositionUpdated -= OnTargetPositionUpdated;
        _targetDetector.LineOfSightLost -= OnLineOfSightLost;
        _targetDetector.TargetLost -= OnTargetLost;
        _chaser.ChaseStarted -= OnChaseStarted;
        _chaser.ChaseStopped -= OnChaseStopped;
        _chaser.TargetReached -= OnTargetReached;
        _fallDetector.Falled -= OnFalled;

        StopForgetTimer();
    }

    private void Update()
    {
        if (!_isAlive) return;

        if (_targetDetector.HasTarget && _targetDetector.HasLineOfSight)
        {
            // Видим игрока - сбрасываем таймер забывания
            ResetForgetTimer();

            if (!_chaser.IsChasing)
            {
                _patrolMover.StopPatrol();
                _chaser.StartChase(_targetDetector.Target.position);
            }
        }
        else if (_targetDetector.HasTarget && !_targetDetector.HasLineOfSight)
        {
            // Не видим игрока, но знаем о нем - таймер забывания тикает
            if (!_isReturningToLastPosition && !_chaser.IsChasing)
            {
                _isReturningToLastPosition = true;
                _chaser.StartChase(_targetDetector.LastKnownPosition);
            }
        }
        else if (!_targetDetector.HasTarget && !_chaser.IsChasing && !_patrolMover.IsPatrolling)
        {
            // Нет цели - патрулируем
            _patrolMover.StartPatrol();
        }
    }

    private void OnTargetDetected(Transform target)
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Target detected! Starting chase.");

        StopForgetTimer();
        _patrolMover.StopPatrol();
        _chaser.StartChase(target.position);
    }

    private void OnTargetPositionUpdated(Vector3 position)
    {
        if (_chaser.IsChasing)
        {
            _chaser.UpdateTarget(position);
            ResetForgetTimer(); // Видим игрока - сбрасываем таймер
        }
    }

    private void OnLineOfSightLost()
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Line of sight lost. Moving to last known position.");

        _isReturningToLastPosition = true;
        _chaser.UpdateTarget(_targetDetector.LastKnownPosition);
        StartForgetTimer();
    }

    private void OnTargetLost()
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Target left vision zone.");

        _isReturningToLastPosition = false;

        // Если цель потеряна полностью, запускаем таймер забывания
        if (_chaser.IsChasing)
        {
            StartForgetTimer();
        }
    }

    private void OnChaseStarted()
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Chase started.");
    }

    private void OnChaseStopped()
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Chase stopped.");

        StopForgetTimer();
        _isReturningToLastPosition = false;

        if (_isAlive && !_targetDetector.HasTarget)
        {
            _patrolMover.StartPatrol();
        }
    }

    private void OnTargetReached()
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Target position reached.");

        if (_isReturningToLastPosition)
        {
            // Достигли последней известной позиции, но игрока не нашли
            // Запускаем таймер забывания
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

    private System.Collections.IEnumerator ForgetTimerCoroutine()
    {
        _forgetTimer = 0;

        while (_forgetTimer < _forgetDelay)
        {
            _forgetTimer += Time.deltaTime;
            yield return null;
        }

        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Forgot about target after {_forgetDelay} seconds.");

        ForgetTarget();
    }

    private void ForgetTarget()
    {
        StopForgetTimer();
        _isReturningToLastPosition = false;
        _chaser.StopChase();

        // Возвращаемся к патрулированию
        if (_isAlive)
        {
            _patrolMover.StartPatrol();
        }
    }

    public void Init(IReadOnlyList<Transform> patrolPoints)
    {
        _patrolMover.SetPatrolPoints(patrolPoints);

        if (!_targetDetector.HasTarget)
        {
            _patrolMover.StartPatrol();
        }
    }

    private void OnFalled()
    {
        _isAlive = false;
        StopForgetTimer();
        _patrolMover.StopPatrol();
        _chaser.StopChase();
        Falled?.Invoke(this);
    }
}