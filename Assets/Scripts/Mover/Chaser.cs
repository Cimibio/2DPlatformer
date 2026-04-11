using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Chaser : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _stoppingDistance = 0.5f;
    [SerializeField] private float _updateInterval = 0.1f;
    [SerializeField] private LayerMask _obstacleLayer;

    private Rigidbody2D _rigidbody;
    private IChasable _currentTarget;
    private bool _isChasing;
    private float _updateTimer;
    private Vector3 _lastTargetPosition;

    public event Action ChaseStarted;
    public event Action ChaseStopped;
    public event Action TargetReached;

    public bool IsChasing => _isChasing;
    public IChasable CurrentTarget => _currentTarget;
    public bool HasLineOfSight { get; private set; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!_isChasing || _currentTarget == null || !_currentTarget.IsAlive)
        {
            if (_isChasing)
                StopChase();
            return;
        }

        _updateTimer += Time.deltaTime;

        if (_updateTimer >= _updateInterval)
        {
            _updateTimer = 0;
            UpdateChase();
        }

        MoveTowardsTarget();
    }

    public void StartChase(IChasable target)
    {
        if (target == null || !target.IsAlive)
            return;

        _currentTarget = target;
        _isChasing = true;
        _lastTargetPosition = target.transform.position;
        ChaseStarted?.Invoke();

        UpdateChase();
    }

    public void StopChase()
    {
        _isChasing = false;
        _currentTarget = null;
        _rigidbody.velocity = Vector2.zero;
        ChaseStopped?.Invoke();
    }

    public void UpdateChase()
    {
        if (!_isChasing || _currentTarget == null)
            return;

        _lastTargetPosition = _currentTarget.transform.position;
        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        if (_currentTarget == null)
        {
            HasLineOfSight = false;
            return;
        }

        Vector2 direction = _currentTarget.transform.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            distance,
            _obstacleLayer
        );

        HasLineOfSight = hit.collider == null;

        if (!HasLineOfSight)
        {
            // ≈сли потер€ли видимость, двигаемс€ к последней известной позиции
            _lastTargetPosition = transform.position * direction.normalized * (distance * 0.5f);
        }
    }

    private void MoveTowardsTarget()
    {
        if (_currentTarget == null)
            return;

        Vector3 targetPosition = HasLineOfSight
            ? _currentTarget.transform.position
            : _lastTargetPosition;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= _stoppingDistance)
        {
            TargetReached?.Invoke();

            if (!HasLineOfSight)
            {
                StopChase();
            }
            return;
        }

        Vector2 direction = (targetPosition - transform.position).normalized;
        _rigidbody.velocity = new Vector2(direction.x * _speed, _rigidbody.velocity.y);

        // ѕоворот в сторону движени€
        if (direction.x != 0)
        {
            transform.rotation = Quaternion.Euler(0, direction.x > 0 ? 0 : 180, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_isChasing && _currentTarget != null)
        {
            Gizmos.color = HasLineOfSight ? Color.green : Color.yellow;
            Gizmos.DrawLine(transform.position, _currentTarget.transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stoppingDistance);
        }
    }
}