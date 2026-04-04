using UnityEngine;


[RequireComponent(typeof(Animator), typeof(PatrolMover))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;
    private PatrolMover _mover;
    private Rigidbody2D _rigidbody;

    private readonly int _speedXHash = Animator.StringToHash("speedX");

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
}
