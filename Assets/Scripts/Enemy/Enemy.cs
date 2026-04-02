using System;
using UnityEngine;

[RequireComponent(typeof(PatrolMover))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _wayPoints;

    private PatrolMover _mover;
    private int _placeIndex = 0;
    private Transform[] _places;

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
        if (_wayPoints == null || _wayPoints.childCount == 0)
            return;

        _places = new Transform[_wayPoints.childCount];

        for (int i = 0; i < _places.Length; i++)
            _places[i] = _wayPoints.GetChild(i);

        SendToCurrentPlace();
    }

    private void OnDisable()
    {
        _mover.PointReached -= GetNextPlace;
    }

    private void GetNextPlace()
    {
        _placeIndex = ++_placeIndex % _places.Length;
        SendToCurrentPlace();
    }

    private void SendToCurrentPlace()
    {
        if (_places.Length > 0)
            _mover.SetMovePoint(_places[_placeIndex].position);
    }
}