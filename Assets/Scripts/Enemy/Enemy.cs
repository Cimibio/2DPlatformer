using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMover), typeof(Chaser), typeof(TargetDetector))]
[RequireComponent(typeof(FallDetector))]
public class Enemy : MonoBehaviour, IChasable
{
    private PatrolMover _patrolMover;
    private Chaser _chaser;
    private TargetDetector _targetDetector;
    private FallDetector _fallDetector;

    private bool _isAlive = true;

    public event Action<Enemy> Falled;
    public bool IsAlive => _isAlive;

    private void Awake()
    {
        _patrolMover = GetComponent<PatrolMover>();
        _chaser = GetComponent<Chaser>();
        _targetDetector = GetComponent<TargetDetector>();
        _fallDetector = GetComponent<FallDetector>();
    }

    private void OnEnable()
    {
        _targetDetector.TargetEntered += OnTargetDetected;
        _targetDetector.TargetExited += OnTargetLost;
        _chaser.ChaseStopped += OnChaseStopped;
        _fallDetector.Falled += OnFalled;
    }

    private void OnDisable()
    {
        _targetDetector.TargetEntered -= OnTargetDetected;
        _targetDetector.TargetExited -= OnTargetLost;
        _chaser.ChaseStopped -= OnChaseStopped;
        _fallDetector.Falled -= OnFalled;
    }

    private void Update()
    {
        if (!_isAlive) return;

        // Основная логика выбора поведения
        if (_targetDetector.HasTarget)
        {
            // Если цель есть - преследуем
            if (!_chaser.IsChasing)
            {
                _patrolMover.StopPatrol();
                _chaser.StartChase(this);
            }
        }
        else
        {
            // Если цели нет - патрулируем
            if (!_patrolMover.IsPatrolling && !_chaser.IsChasing)
            {
                _patrolMover.StartPatrol();
            }
        }
    }

    private void OnTargetDetected(Transform target)
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Target detected, starting chase!");

        _patrolMover.StopPatrol();
        _chaser.StartChase(this);
    }

    private void OnTargetLost()
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Target lost, returning to patrol!");

        // Не останавливаем чейзер сразу, он сам решит когда остановиться
    }

    private void OnChaseStopped()
    {
        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Chase stopped, resuming patrol!");

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
        _patrolMover.StopPatrol();
        _chaser.StopChase();
        Falled?.Invoke(this);
    }

    private bool _debugMode = true;
}