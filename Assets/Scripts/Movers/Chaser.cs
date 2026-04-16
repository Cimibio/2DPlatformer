using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Chaser : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _stoppingDistance = 0.5f;
    [SerializeField] private bool _rotateToTarget = true;

    private Rigidbody2D _rigidbody;
    private Vector3 _currentTarget;
    private bool _isChasing;
    private float _sqrStoppingDistance;

    public event Action ChaseStarted;
    public event Action ChaseStopped;
    public event Action TargetReached;

    public bool IsChasing => _isChasing;
    public Vector3 CurrentTarget => _currentTarget;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
    }

    private void Update()
    {
        if (!_isChasing) return;

        float sqrDistance = (transform.position - _currentTarget).sqrMagnitude;

        if (sqrDistance <= _sqrStoppingDistance)
        {
            StopChase();
            TargetReached?.Invoke();
            return;
        }

        MoveTowardsTarget();
    }

    public void Chase(Vector3 targetPosition)
    {
        _isChasing = true;
        _currentTarget = targetPosition;
        ChaseStarted?.Invoke();
    }

    public void UpdateTarget(Vector3 newTargetPosition)
    {
        if (_isChasing)
        {
            _currentTarget = newTargetPosition;
        }
    }

    public void StopChase()
    {
        _isChasing = false;
        _rigidbody.velocity = Vector2.zero;
        ChaseStopped?.Invoke();
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (_currentTarget - transform.position).normalized;
        _rigidbody.velocity = new Vector2(direction.x * _speed, _rigidbody.velocity.y);

        if (_rotateToTarget && direction.x != 0)
        {
            transform.rotation = Quaternion.Euler(0, direction.x > 0 ? 0 : 180, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_isChasing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _currentTarget);
            Gizmos.DrawWireSphere(_currentTarget, _stoppingDistance);
        }
    }
}