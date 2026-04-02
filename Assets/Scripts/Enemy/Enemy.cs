using System;
using UnityEngine;

[RequireComponent(typeof(PatrolMover))]
public class Enemy : MonoBehaviour
{
    

    private PatrolMover _mover;
    private int _placeIndex = 0;
    private Transform[] _points;

    //public event Action Falled;

    private void Awake()
    {
        _mover = GetComponent<PatrolMover>();
    }
    private void OnEnable()
    {
        _mover.PointReached += GetNextPlace;
    }

    private void Start()
    {
        
    }

    private void OnDisable()
    {
        _mover.PointReached -= GetNextPlace;
    }

    public void Init(Transform _patrolPoints)
    {
        if (_patrolPoints == null || _patrolPoints.childCount == 0)
            return;

        _points = new Transform[_patrolPoints.childCount];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = _patrolPoints.GetChild(i);

        SendToCurrentPlace();
    }

    private void GetNextPlace()
    {
        _placeIndex = ++_placeIndex % _points.Length;
        SendToCurrentPlace();
    }

    private void SendToCurrentPlace()
    {
        if (_points.Length > 0)
            _mover.SetMovePoint(_points[_placeIndex].position);
    }
}