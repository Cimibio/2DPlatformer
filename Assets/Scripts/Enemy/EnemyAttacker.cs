using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _damage = 10;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private bool _debugMode = true;

    private Transform _currentTarget;
    private Health _currentTargetHealth;
    private bool _isAttackCharged = true;
    private float _cooldownTimer;

    public bool CanAttack
    {
        get
        {
            if (!_isAttackCharged || _currentTarget == null)
                return false;

            return _currentTargetHealth != null && _currentTargetHealth.IsAlive;
        }
    }

    private void Update()
    {
        if (!_isAttackCharged)
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0)
            {
                _isAttackCharged = true;
            }
        }
    }

    public void SetTarget(Transform target)
    {
        _currentTarget = target;

        if (target != null)
            target.TryGetComponent(out _currentTargetHealth);
        else
            _currentTargetHealth = null;
    }

    public void ClearTarget()
    {
        _currentTarget = null;
        _currentTargetHealth = null;
    }

    public bool IsTargetInAttackRange()
    {
        if (_currentTarget == null)
            return false;

        Vector2 offset = (Vector2)_currentTarget.position - (Vector2)transform.position;
        return offset.sqrMagnitude <= _attackRange * _attackRange;
    }

    public void Attack()
    {
        if (!CanAttack || _currentTargetHealth == null)
            return;

        if (IsTargetInAttackRange())
            _currentTargetHealth.TakeDamage(_damage);

        if (_debugMode)
            Debug.Log($"[{gameObject.name}] Attacked {_currentTarget.name} for {_damage} damage!");

        _isAttackCharged = false;
        _cooldownTimer = _attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isAttackCharged ? Color.red : Color.gray;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}