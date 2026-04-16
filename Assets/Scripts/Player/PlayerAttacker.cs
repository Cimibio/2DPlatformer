using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
public class PlayerAttacker : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _attackWidth = 1f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private bool _debugMode = true;

    private float _damage = 0;
    private bool _isAttackCharged = true;
    private float _cooldownTimer;
    private IDamageable _player;
    private PlayerInputReader _inputReader;
    private Transform _transform;

    public bool CanAttack => _isAttackCharged && _player.IsAlive;
    public bool IsAttacking { get; private set; }

    public event Action AttackStarted;
    public event Action<IDamageable> AttackHit;
    public event Action AttackMissed;
    public event Action AttackCompleted;

    private void Awake()
    {
        _transform = transform;        
        _inputReader = GetComponent<PlayerInputReader>();

        if (TryGetComponent(out IDamageable damageable))
        {
            _player = damageable;            
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] PlayerAttacker requires Player component!");
            enabled = false;
            return;
        }

        if (TryGetComponent(out IAttackable attacker))
        {
            _damage = attacker.Damage;
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Requires IAttackable component!");
            enabled = false;
        }
    }

    private void Update()
    {
        ChargeAttack();

        // Проверка ввода через InputReader
        if (CanAttack && _inputReader.IsAttackPressed)
        {
            PerformAttack();
        }
    }

    private void ChargeAttack()
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

    private void PerformAttack()
    {
        IsAttacking = true;
        AttackStarted?.Invoke();

        // Определяем направление атаки
        Vector2 attackDirection = GetAttackDirection();

        // Находим цель
        IDamageable hitTarget = FindTargetInAttackZone(attackDirection);

        if (hitTarget != null)
        {
            // Попали по цели
            hitTarget.TakeDamage(_damage);
            AttackHit?.Invoke(hitTarget);

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Hit {hitTarget} for {_damage} damage!");
        }
        else
        {
            // Промах
            AttackMissed?.Invoke();

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Attack missed!");
        }

        // Запускаем кулдаун
        _isAttackCharged = false;
        _cooldownTimer = _attackCooldown;

        StartCoroutine(ResetAttackFlag());
    }

    private Vector2 GetAttackDirection()
    {
        // Определяем направление по повороту спрайта
        if (Mathf.Approximately(_transform.localScale.x, 1))
            return Vector2.right;
        else
            return Vector2.left;
    }

    private IDamageable FindTargetInAttackZone(Vector2 direction)
    {
        // BoxCast - атака прямоугольной зоной
        Vector2 boxSize = new Vector2(_attackRange, _attackWidth);
        Vector2 origin = (Vector2)_transform.position + direction * (_attackRange * 0.5f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, boxSize, 0f, direction, 0f, _targetLayer);

        // Ищем первую живую цель
        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;

            if (hit.collider.TryGetComponent(out IDamageable target))
            {
                if (target.IsAlive)
                {
                    return target;
                }
            }
        }

        return null;
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForEndOfFrame();
        IsAttacking = false;
        AttackCompleted?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            DrawAttackZone(Vector2.right);
        }
        else
        {
            DrawAttackZone(GetAttackDirection());
        }
    }

    private void DrawAttackZone(Vector2 direction)
    {
        Gizmos.color = _isAttackCharged ? Color.green : Color.gray;

        Vector2 boxSize = new Vector2(_attackRange, _attackWidth);
        Vector2 center = (Vector2)transform.position + direction * (_attackRange * 0.5f);

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
        Gizmos.matrix = oldMatrix;

        Gizmos.color = _isAttackCharged ? Color.red : Color.gray;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * _attackRange);
    }
}