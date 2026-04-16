using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(PlayerInputReader), typeof(Mover), typeof(FallDetector))]
[RequireComponent(typeof(PlayerAnimator))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _jumpForce = 4f;

    [Header("Combat Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _hitStunDuration = 0.5f;

    private PlayerInputReader _inputReader;
    private Mover _mover;
    private Rigidbody2D _rigidbody;
    private WaitForSeconds _stunDuration;

    private float _currentHealth;
    private bool _isStunned = false;

    public bool IsAlive => _currentHealth > 0;
    public bool CanMove => IsAlive && !_isStunned;

    public event Action Hitted;
    public event Action Died;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _mover = GetComponent<Mover>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _stunDuration = new WaitForSeconds(_hitStunDuration);
        _currentHealth = _maxHealth;
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            _mover.Move(_inputReader.HorizontalInput, _speed);
        }
        else if (_isStunned)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }
    }

    private void Update()
    {
        if (_inputReader.IsJumpPressed)
            _mover.Jump(_jumpForce);
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive)
            return;

        _currentHealth -= damage;

        Hitted?.Invoke();

        Debug.Log($"[Player] Took {damage} damage. Health: {_currentHealth}/{_maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitStunCoroutine());
        }
    }

    public void Reset()
    {
        _currentHealth = _maxHealth;
        _isStunned = false;

        if (_inputReader != null)
            _inputReader.enabled = true;

        if (_mover != null)
            _mover.enabled = true;

        if (_rigidbody != null)
            _rigidbody.velocity = Vector2.zero;

        StopAllCoroutines();
    }

    private IEnumerator HitStunCoroutine()
    {
        _isStunned = true;
        _inputReader.enabled = false;

        yield return _stunDuration;

        _isStunned = false;
        _inputReader.enabled = true;
    }

    private void Die()
    {
        _currentHealth = 0;

        Debug.Log("[Player] Died!");

        Died?.Invoke();
        _inputReader.enabled = false;
        _mover.enabled = false;

        // TODO: Рестарт уровня, экран смерти и т.д.
    }
}