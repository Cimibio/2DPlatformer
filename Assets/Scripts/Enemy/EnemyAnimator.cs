using UnityEngine;
using System;

[RequireComponent(typeof(Animator), typeof(PatrolMover))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;
    private PatrolMover _mover;
    private Rigidbody2D _rigidbody;

    private readonly int _speedXHash = Animator.StringToHash("speedX");
    private readonly int _hitHash = Animator.StringToHash("hit");
    private readonly int _dieHash = Animator.StringToHash("die");

    public event Action EnemyDeathAnimationCompleted;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _mover = GetComponent<PatrolMover>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalSpeed = Mathf.Abs(_rigidbody.velocity.x);
        _animator.SetFloat(_speedXHash, horizontalSpeed);
    }

    public void PlayHitAnimation()
    {
        _animator.SetTrigger(_hitHash);
    }

    public void PlayDieAnimation()
    {
        _animator.SetTrigger(_dieHash);
    }

    public void NotifyDeathAnimationComplete()
    {
        Debug.Log($"EnemyAnimator: Death animation completed on {gameObject.name}");
        EnemyDeathAnimationCompleted?.Invoke();
    }
}
