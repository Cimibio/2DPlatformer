using System;
using UnityEngine;

public class PatrolMover : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;

    private Vector3 _movePointPosition;
    private float _minDistance = 0.2f;

    public event Action PointReached;

    private void Update()
    {
        Move();

        if ((_movePointPosition - transform.position).sqrMagnitude <= _minDistance * _minDistance)
            PointReached?.Invoke();
    }

    public void SetMovePoint(Vector3 target)
    {
        _movePointPosition = target;
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _movePointPosition, _speed * Time.deltaTime);
    }
}
