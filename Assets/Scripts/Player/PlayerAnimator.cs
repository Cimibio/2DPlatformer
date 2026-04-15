using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Mover), typeof(Player))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private Mover _mover;
    private Rigidbody2D _rigidbody;
    private Player _player;

    private readonly int _speedXHash = Animator.StringToHash("speedX");
    private readonly int _speedYHash = Animator.StringToHash("speedY");
    private readonly int _isGroundedHash = Animator.StringToHash("isGrounded");
    private readonly int _hitHash = Animator.StringToHash("hit");
    private readonly int _dieHash = Animator.StringToHash("die");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _mover = GetComponent<Mover>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
    }
    private void OnEnable()
    {
        _player.Hitted += PlayHitAnimation;
        _player.Died += PlayDieAnimation;
    }

    private void OnDisable()
    {
        _player.Hitted -= PlayHitAnimation;
        _player.Died -= PlayDieAnimation;
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
    private void PlayHitAnimation()
    {
        _animator.SetTrigger(_hitHash);
    }

    private void PlayDieAnimation()
    {
        _animator.SetTrigger(_dieHash);
    }
}