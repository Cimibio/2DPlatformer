using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class TargetChaser : MonoBehaviour
{
    [SerializeField] protected float _speed = 3f;
    [SerializeField] protected float _stoppingDistance = 0.5f;
    [SerializeField] protected bool _rotateToTarget = true;

    protected Rigidbody2D _rigidbody;
    protected Vector3 _targetPosition;
    protected bool _isMoving;
    protected float _sqrStoppingDistance;

    public bool IsMoving => _isMoving;
    public Vector3 TargetPosition => _targetPosition;

    public bool IsReached
    {
        get
        {
            if (!_isMoving)
                return false;

            float sqrDistance = (transform.position - _targetPosition).sqrMagnitude;
            return sqrDistance <= _sqrStoppingDistance;
        }
    }

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sqrStoppingDistance = _stoppingDistance * _stoppingDistance;
    }

    protected virtual void Update()
    {
        if (!_isMoving)
            return;

        if (IsReached)
        {
            StopMoving();
            OnReached();
            return;
        }

        MoveTowardsTarget();
    }

    public virtual void MoveTo(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _isMoving = true;
    }

    public virtual void StopMoving()
    {
        _isMoving = false;
        _rigidbody.velocity = Vector2.zero;
    }

    public virtual void UpdateTarget(Vector3 newTargetPosition)
    {
        if (_isMoving)
            _targetPosition = newTargetPosition;
    }

    protected virtual void MoveTowardsTarget()
    {
        Vector2 direction = (_targetPosition - transform.position).normalized;
        _rigidbody.velocity = new Vector2(direction.x * _speed, _rigidbody.velocity.y);

        if (_rotateToTarget && direction.x != 0)
            transform.rotation = Quaternion.Euler(0, direction.x > 0 ? 0 : 180, 0);
    }

    protected virtual void OnReached() { }

    protected virtual void OnDrawGizmosSelected()
    {
        if (_isMoving)
        {
            Gizmos.color = GetGizmoColor();
            Gizmos.DrawLine(transform.position, _targetPosition);
            Gizmos.DrawWireSphere(_targetPosition, _stoppingDistance);
        }
    }

    protected abstract Color GetGizmoColor();
}