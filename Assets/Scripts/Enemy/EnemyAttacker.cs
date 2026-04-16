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
    private IDamageable _currentDamageable;
    private bool _isAttackCharged = true;
    private float _cooldownTimer;

    public bool CanAttack
    {
        get
        {
            if (!_isAttackCharged || _currentTarget == null)
                return false;

            return _currentDamageable != null && _currentDamageable.IsAlive;
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
            target.TryGetComponent(out _currentDamageable);
        else
            _currentDamageable = null;
    }

    public void ClearTarget()
    {
        _currentTarget = null;
        _currentDamageable = null;
    }

    public bool IsTargetInAttackRange()
    {
        if (_currentTarget == null)
            return false;

        float distance = Vector2.Distance(transform.position, _currentTarget.position);
        return distance <= _attackRange;
    }

    public void Attack()
    {
        if (!CanAttack || _currentDamageable == null)
            return;

        if (IsTargetInAttackRange())
            _currentDamageable.TakeDamage(_damage);

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