using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMover : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _waypointTolerance = 0.2f;

    private List<Transform> _patrolPoints = new List<Transform>();
    private int _currentPointIndex = 0;
    private Vector3 _currentTarget;
    private bool _isPatrolling;
    private bool _isWaiting;

    public event Action PointReached;
    public event Action PatrolStarted;
    public event Action PatrolStopped;

    public bool IsPatrolling => _isPatrolling;
    public int CurrentWaypointIndex => _currentPointIndex;

    private void Update()
    {
        if (!_isPatrolling || _isWaiting)
            return;

        Move();
        CheckWaypointReached();
    }

    public void SetPatrolPoints(IReadOnlyList<Transform> points)
    {
        _patrolPoints.Clear();

        if (points == null || points.Count == 0)
            return;

        foreach (var point in points)
        {
            if (point != null)
                _patrolPoints.Add(point);
        }

        if (_patrolPoints.Count > 0)
        {
            _currentPointIndex = 0;
            _currentTarget = _patrolPoints[0].position;
        }
    }

    public void StartPatrol()
    {
        if (_patrolPoints.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] No patrol points assigned!");
            return;
        }

        _isPatrolling = true;
        _isWaiting = false;
        PatrolStarted?.Invoke();
    }

    public void StopPatrol()
    {
        _isPatrolling = false;
        _isWaiting = false;
        PatrolStopped?.Invoke();
    }

    public void UpdatePatrol()
    {
        // Для совместимости с интерфейсом
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _currentTarget,
            _speed * Time.deltaTime
        );
    }

    private void CheckWaypointReached()
    {
        float distance = Vector3.Distance(transform.position, _currentTarget);

        if (distance <= _waypointTolerance)
        {
            PointReached?.Invoke();
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        if (_patrolPoints.Count == 0)
            return;

        _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Count;
        _currentTarget = _patrolPoints[_currentPointIndex].position;
    }

    // Опционально: добавить задержку на точках
    public void WaitAtWaypoint(float duration)
    {
        if (duration <= 0)
            return;

        _isWaiting = true;
        Invoke(nameof(ResumePatrol), duration);
    }

    private void ResumePatrol()
    {
        _isWaiting = false;
    }
}