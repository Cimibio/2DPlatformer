using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Mover))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private Mover _mover;
    private Rigidbody2D _rigidbody;

    private static readonly int SpeedX = Animator.StringToHash("speedX");
    private static readonly int SpeedY = Animator.StringToHash("speedY");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

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

        // В воздухе отключаем горизонтальную анимацию
        float targetSpeedX = isGrounded ? horizontalSpeed : 0f;

        _animator.SetFloat(SpeedX, targetSpeedX);
        _animator.SetFloat(SpeedY, verticalVelocity);
        _animator.SetBool(IsGrounded, isGrounded);

        // Отладка
        // Debug.Log($"SpeedX: {targetSpeedX:F2}, SpeedY: {verticalVelocity:F2}, IsGrounded: {isGrounded}");
    }
}