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
        // Получаем горизонтальную скорость
        float horizontalSpeed = Mathf.Abs(_rigidbody.velocity.x);
        float verticalSpeed = _rigidbody.velocity.y;
        bool isGrounded = _mover.IsGrounded();

        // Устанавливаем параметры для Blend Tree
        _animator.SetFloat(SpeedX, horizontalSpeed);
        _animator.SetFloat(SpeedY, isGrounded ? 0 : verticalSpeed);
        _animator.SetBool(IsGrounded, isGrounded);

        // Отладка (опционально)
        // Debug.Log($"SpeedX: {horizontalSpeed}, SpeedY: {verticalSpeed}, Grounded: {isGrounded}");
    }
}