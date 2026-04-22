using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private readonly int _speedXHash = Animator.StringToHash("speedX");
    private readonly int _speedYHash = Animator.StringToHash("speedY");
    private readonly int _isGroundedHash = Animator.StringToHash("isGrounded");
    private readonly int _hitHash = Animator.StringToHash("hurt");
    private readonly int _dieHash = Animator.StringToHash("die");
    private readonly int _attackHash = Animator.StringToHash("attack");

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateMovementAnimation(float horizontalSpeed, float verticalVelocity, bool isGrounded)
    {
        float targetSpeedX = isGrounded ? Mathf.Abs(horizontalSpeed) : 0f;

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