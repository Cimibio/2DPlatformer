using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Mover))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private Mover _mover;
    private Rigidbody2D _rigidbody;

    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int VerticalSpeed = Animator.StringToHash("verticalSpeed");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _mover = GetComponent<Mover>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Обновляем параметры аниматора
        _animator.SetBool(IsRunning, Mathf.Abs(_rigidbody.velocity.x) > 0.1f);
        _animator.SetBool(IsGrounded, _mover.IsGrounded());
        _animator.SetFloat(VerticalSpeed, _rigidbody.velocity.y);
    }
}