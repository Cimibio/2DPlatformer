using UnityEngine;

[RequireComponent(typeof(PlayerInputReader), typeof(Mover), typeof(FallDetector))]
[RequireComponent (typeof(PlayerAnimator))]
public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _jumpForce = 12f;

    private PlayerInputReader _inputReader;
    private Mover _mover;
    private FallDetector _fallDetector;
    private PlayerAnimator _playerAnimator;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _mover = GetComponent<Mover>();
        _fallDetector = GetComponent<FallDetector>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        _mover.Move(_inputReader.HorizontalInput, _speed);

        if (_inputReader.IsJumpPressed)
        {
            _mover.Jump(_jumpForce);
        }
    }
}