using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Mover))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private Mover _mover;
    private Rigidbody2D _rigidbody;

    private readonly int _speedXHash = Animator.StringToHash("speedX");
    private readonly int _speedYHash = Animator.StringToHash("speedY");
    private readonly int _isGroundedHash = Animator.StringToHash("isGrounded");

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
}