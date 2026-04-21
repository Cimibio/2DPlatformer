using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Searcher : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _reachDistance = 0.2f;
    [SerializeField] private bool _rotateToTarget = true;

    private Rigidbody2D _rigidbody;
    private Vector3 _targetPosition;
    private bool _isSearching;
    private float _sqrReachDistance;

    public bool IsSearching => _isSearching;
    public bool IsReached { get; private set; }
    public Vector3 TargetPosition => _targetPosition;

    public event Action Reached;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sqrReachDistance = _reachDistance * _reachDistance;
    }

    private void Update()
    {
        if (!_isSearching)
            return;

        if (IsReached)
            return;

        float sqrDistance = (transform.position - _targetPosition).sqrMagnitude;

        if (sqrDistance <= _sqrReachDistance)
        {
            IsReached = true;
            StopSearch();
            Reached?.Invoke();
            return;
        }

        MoveTowardsTarget();
    }

    public void StartSearch(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _isSearching = true;
        IsReached = false;
    }

    public void StopSearch()
    {
        _isSearching = false;
        _rigidbody.velocity = Vector2.zero;
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (_targetPosition - transform.position).normalized;
        _rigidbody.velocity = new Vector2(direction.x * _speed, _rigidbody.velocity.y);

        if (_rotateToTarget && direction.x != 0)
            transform.rotation = Quaternion.Euler(0, direction.x > 0 ? 0 : 180, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (_isSearching)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _targetPosition);
            Gizmos.DrawWireSphere(_targetPosition, _reachDistance);
        }
    }
}