using UnityEngine;

[RequireComponent(typeof(PlayerInputReader), typeof(Mover), typeof(FallDetector))]
[RequireComponent(typeof(PlayerAnimator))]
public class Player : MonoBehaviour, IChasable
{
    [Header("Settings")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _jumpForce = 4f;

    private PlayerInputReader _inputReader;
    private Mover _mover;
    private FallDetector _fallDetector;

    public bool IsAlive => true;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _mover = GetComponent<Mover>();
        _fallDetector = GetComponent<FallDetector>();
    }

    private void FixedUpdate()
    {
        _mover.Move(_inputReader.HorizontalInput, _speed);
    }

    private void Update()
    {
        if (_inputReader.IsJumpPressed)
            _mover.Jump(_jumpForce);
    }
}