using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private bool _debugMode = true;

    private Enemy _enemy;
    private Transform _currentTarget;
    private bool _canAttack = true;
    private float _cooldownTimer;

    public bool CanAttack => _canAttack && _currentTarget != null;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (!_canAttack)
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0)
            {
                _canAttack = true;
            }
        }
    }

    public void SetTarget(Transform target)
    {
        _currentTarget = target;
    }

    public void ClearTarget()
    {
        _currentTarget = null;
    }

    public bool IsTargetInAttackRange()
    {
        if (_currentTarget == null) return false;

        float distance = Vector2.Distance(transform.position, _currentTarget.position);
        return distance <= _attackRange;
    }

    public void Attack()
    {
        if (!CanAttack || _currentTarget == null || !_enemy.IsAlive)
            return;

        if (!IsTargetInAttackRange())
            return;

        if (!_currentTarget.TryGetComponent(out IDamageable target))
            return;

        if (!target.IsAlive)
            return;

        // Наносим урон
        target.TakeDamage(_enemy.Damage);

        if (_debugMode)        
            Debug.Log($"[{gameObject.name}] Attacked {_currentTarget.name} for {_enemy.Damage} damage!");        

        // Запускаем кулдаун
        _canAttack = false;
        _cooldownTimer = _attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
