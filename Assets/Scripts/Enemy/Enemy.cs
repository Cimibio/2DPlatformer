using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMover), typeof(FallDetector))]
public class Enemy : MonoBehaviour
{
    private PatrolMover _mover;
    private int _patrolPointIndex = 0;
    private List<Transform> _patrolPoints = new List<Transform>();
    private FallDetector _fallDetector;

    public event Action<Enemy> Falled;

    private void Awake()
    {
        _mover = GetComponent<PatrolMover>();
        _fallDetector = GetComponent<FallDetector>();
    }

    private void OnEnable()
    {
        _mover.PointReached += GetNextPlace;
        _fallDetector.Falled += Falling;
    }

    private void OnDisable()
    {
        _mover.PointReached -= GetNextPlace;
        _fallDetector.Falled -= Falling;
    }

    public void Init(IReadOnlyList<Transform> patrolPoints)
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
            return;

        _patrolPoints.Clear();
        _patrolPointIndex = 0;

        foreach (var item in patrolPoints)
            _patrolPoints.Add(item);

        SendToCurrentPlace();
    }

    private void Falling()
    {
        Falled?.Invoke(this);
    }

    private void GetNextPlace()
    {
        _patrolPointIndex = ++_patrolPointIndex % _patrolPoints.Count;
        SendToCurrentPlace();
    }

    private void SendToCurrentPlace()
    {
        if (_patrolPoints.Count > 0)
            _mover.SetMovePoint(_patrolPoints[_patrolPointIndex].position);
    }
}