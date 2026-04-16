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
    private float _sqrWaypointTolerance;

    public event Action PointReached;
    public event Action PatrolStarted;
    public event Action PatrolStopped;

    public bool IsPatrolling => _isPatrolling;
    public int CurrentWaypointIndex => _currentPointIndex;

    private void Awake()
    {
        _sqrWaypointTolerance = _waypointTolerance * _waypointTolerance;
    }

    private void Update()
    {
        if (!_isPatrolling || _isWaiting)
            return;

        Move();

        if (IsWaypointReached())
        {
            PointReached?.Invoke();
            MoveToNextWaypoint();
        }
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

    public void Patrol()
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

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, _speed * Time.deltaTime);
    }

    private bool IsWaypointReached()
    {
        float sqrDistance = (transform.position - _currentTarget).sqrMagnitude;

        return sqrDistance <= _sqrWaypointTolerance;
    }

    private void MoveToNextWaypoint()
    {
        if (_patrolPoints.Count == 0)
            return;

        _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Count;
        _currentTarget = _patrolPoints[_currentPointIndex].position;
    }
}