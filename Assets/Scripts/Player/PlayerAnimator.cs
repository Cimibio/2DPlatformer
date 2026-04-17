using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Mover), typeof(Player))]
public class PlayerAnimator : MonoBehaviour
{
    private readonly int _speedXHash = Animator.StringToHash("speedX");
    private readonly int _speedYHash = Animator.StringToHash("speedY");
    private readonly int _isGroundedHash = Animator.StringToHash("isGrounded");
    private readonly int _hitHash = Animator.StringToHash("hurt");
    private readonly int _dieHash = Animator.StringToHash("die");
    private readonly int _attackHash = Animator.StringToHash("attack");

    private Animator _animator;
    private Mover _mover;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _mover = GetComponent<Mover>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalSpeed = Mathf.Abs(_rigidbody.velocity.x);
        float verticalVelocity = _rigidbody.velocity.y;
        bool isGrounded = _mover.IsGrounded();

        float targetSpeedX = isGrounded ? horizontalSpeed : 0f;

        _animator.SetFloat(_speedXHash, targetSpeedX);
        _animator.SetFloat(_speedYHash, verticalVelocity);
        _animator.SetBool(_isGroundedHash, isGrounded);
    }

    public void PlayHitAnimation()
    {
        _animator.SetTrigger(_hitHash);
    }

    public void PlayDieAnimation()
    {
        _animator.SetTrigger(_dieHash);
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger(_attackHash);
    }
}