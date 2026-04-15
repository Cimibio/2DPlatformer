using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(PlayerInputReader), typeof(Mover), typeof(FallDetector))]
[RequireComponent(typeof(PlayerAnimator))]
public class Player : MonoBehaviour, IChasable, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _jumpForce = 4f;

    [Header("Combat Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _hitStunDuration = 0.5f;

    private PlayerInputReader _inputReader;
    private Mover _mover;
    private FallDetector _fallDetector;
    private PlayerAnimator _animator;
    private Rigidbody2D _rigidbody;
    private WaitForSeconds _stunDuration;

    private float _currentHealth;
    private bool _isAlive = true;
    private bool _isStunned = false;

    public bool IsAlive => _isAlive;
    public bool CanMove => _isAlive && !_isStunned;

    public event Action Hitted;
    public event Action Died;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _mover = GetComponent<Mover>();
        _fallDetector = GetComponent<FallDetector>();
        _animator = GetComponent<PlayerAnimator>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _stunDuration = new WaitForSeconds(_hitStunDuration);
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            _mover.Move(_inputReader.HorizontalInput, _speed);
        }
        else if (_isStunned)
        {
            // Останавливаем движение при стане
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
        if (!_isAlive) 
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

    private IEnumerator HitStunCoroutine()
    {
        _isStunned = true;

        // Проигрываем анимацию получения урона
        //_animator.PlayHitAnimation();

        // Блокируем ввод
        _inputReader.enabled = false;

        yield return _stunDuration;

        //_isStunned = false;
        _inputReader.enabled = true;
    }

    private void Die()
    {
        _isAlive = false;
        _currentHealth = 0;

        Debug.Log("[Player] Died!");

        Died?.Invoke();
        //_animator.PlayDieAnimation();
        _inputReader.enabled = false;
        _mover.enabled = false;

        // TODO: Рестарт уровня, экран смерти и т.д.
    }
}