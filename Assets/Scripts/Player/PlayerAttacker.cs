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
    [SerializeField] private float _damage = 30f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private bool _debugMode = true;

    private bool _isAttackCharged = true;
    private float _cooldownTimer;
    private PlayerInputReader _inputReader;

    public bool CanAttack => _isAttackCharged;
    public bool IsAttacking { get; private set; }

    public event Action AttackStarted;

    private void Awake()
    {      
        _inputReader = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        ChargeAttack();

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

        Vector2 attackDirection = GetAttackDirection();
        IDamageable hitTarget = FindTargetInAttackZone(attackDirection);

        if (hitTarget != null)
        {
            hitTarget.TakeDamage(_damage);

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Hit {hitTarget} for {_damage} damage!");
        }
        else
        {
            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Attack missed!");
        }

        _isAttackCharged = false;
        _cooldownTimer = _attackCooldown;

        StartCoroutine(ResetAttackFlag());
    }

    private Vector2 GetAttackDirection()
    {
        if (TryGetComponent(out Mover mover))
        {
            return mover.IsFacingRight ? Vector2.right : Vector2.left;
        }

        return transform.rotation.y == 0 ? Vector2.right : Vector2.left;
    }

    private IDamageable FindTargetInAttackZone(Vector2 direction)
    {
        Vector2 center = (Vector2)transform.position + direction * (_attackRange * 0.5f);
        Vector2 boxSize = new Vector2(_attackRange, _attackWidth);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f, _targetLayer);

        foreach (var hit in hits)
        {
            if (hit == null) 
                continue;

            if (hit.isTrigger) 
                continue;

            if (hit.TryGetComponent(out IDamageable target) && target.IsAlive)
            {
                return target;
            }
        }

        return null;
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForEndOfFrame();
        IsAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)        
            DrawAttackZone(Vector2.right);        
        else        
            DrawAttackZone(GetAttackDirection());        
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