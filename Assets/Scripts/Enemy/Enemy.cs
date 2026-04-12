using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMover), typeof(Chaser), typeof(TargetDetector))]
[RequireComponent(typeof(FallDetector))]
public class Enemy : MonoBehaviour
{
    private PatrolMover _patrolMover;
    private Chaser _chaser;
    private TargetDetector _targetDetector;
    private FallDetector _fallDetector;

    private bool _isAlive = true;
    private bool _isReturningToLastPosition = false;

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
        _chaser.ChaseStopped += OnChaseStopped;
        _fallDetector.Falled += OnFalled;
    }

    private void OnDisable()
    {
        _targetDetector.TargetDetected -= OnTargetDetected;
        _targetDetector.TargetPositionUpdated -= OnTargetPositionUpdated;
        _targetDetector.LineOfSightLost -= OnLineOfSightLost;
        _targetDetector.TargetLost -= OnTargetLost;
        _chaser.ChaseStopped -= OnChaseStopped;
        _fallDetector.Falled -= OnFalled;
    }

    private void Update()
    {
        if (!_isAlive) return;

        if (_targetDetector.HasTarget && _targetDetector.HasLineOfSight)
        {
            // Видим игрока - преследуем
            if (!_chaser.IsChasing)
            {
                _patrolMover.StopPatrol();
                _chaser.StartChase(_targetDetector.Target.position);
            }
        }
        else if (_targetDetector.HasTarget && !_targetDetector.HasLineOfSight && !_isReturningToLastPosition)
        {
            // Потеряли видимость - идем к последней известной позиции
            _isReturningToLastPosition = true;
            _chaser.UpdateTarget(_targetDetector.LastKnownPosition);
        }
        else if (!_targetDetector.HasTarget && !_chaser.IsChasing && !_patrolMover.IsPatrolling)
        {
            // Нет цели - патрулируем
            _patrolMover.StartPatrol();
        }
    }

    private void OnTargetDetected(Transform target)
    {
        _patrolMover.StopPatrol();
        _chaser.StartChase(target.position);
    }

    private void OnTargetPositionUpdated(Vector3 position)
    {
        if (_chaser.IsChasing)
        {
            _chaser.UpdateTarget(position);
        }
    }

    private void OnLineOfSightLost()
    {
        _isReturningToLastPosition = true;
        _chaser.UpdateTarget(_targetDetector.LastKnownPosition);
    }

    private void OnTargetLost()
    {
        _isReturningToLastPosition = false;
    }

    private void OnChaseStopped()
    {
        if (_isAlive && !_targetDetector.HasTarget)
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
        _patrolMover.StopPatrol();
        _chaser.StopChase();
        Falled?.Invoke(this);
    }
}