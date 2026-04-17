using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(PlayerInputReader), typeof(Mover), typeof(FallDetector))]
[RequireComponent(typeof(PlayerAnimator), typeof(Health))]
public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _jumpForce = 4f;

    [Header("Combat Settings")]
    [SerializeField] private float _hitStunDuration = 0.5f;

    private PlayerInputReader _inputReader;
    private Mover _mover;
    private Rigidbody2D _rigidbody;
    private WaitForSeconds _stunDuration;
    private PlayerAnimator _animator;
    private Health _health;

    private int _score = 0;
    private bool _isStunned = false;

    public bool IsAlive => _health.IsAlive;
    public bool CanMove => IsAlive && !_isStunned;
    public int Score => _score;
    public Health Health => _health;

    public event Action Died;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _mover = GetComponent<Mover>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<PlayerAnimator>();
        _health = GetComponent<Health>();

        _stunDuration = new WaitForSeconds(_hitStunDuration);
    }

    private void OnEnable()
    {
        _health.Died += Die;
        _health.Hitted += Stun;
    }

    private void OnDisable()
    {
        _health.Died -= Die;
        _health.Hitted -= Stun;
    }

    private void FixedUpdate()
    {
        if (CanMove)        
            _mover.Move(_inputReader.HorizontalInput, _speed);        
        else if (_isStunned)        
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);        
    }

    private void Update()
    {
        if (CanMove && _inputReader.IsJumpPressed)
            _mover.Jump(_jumpForce);
    }

    public void Reset()
    {
        _health.Reset();
        _isStunned = false;
        _score = 0;

        if (_inputReader != null)
            _inputReader.enabled = true;

        if (_mover != null)
            _mover.enabled = true;

        if (_rigidbody != null)
            _rigidbody.velocity = Vector2.zero;

        StopAllCoroutines();
    }

    public void AddScore(int amount)
    {
        _score += amount;
        Debug.Log($"[Player] Score increased by {amount}. Total score: {_score}");
    }

    private void Stun()
    {
        StartCoroutine(HitStunCoroutine());
        _animator.PlayHitAnimation();
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
        Debug.Log("[Player] Died!");

        Died?.Invoke();
        _animator.PlayDieAnimation();
        _inputReader.enabled = false;
        _mover.enabled = false;
    }
}