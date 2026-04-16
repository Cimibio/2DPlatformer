using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private float _zOffset = -10f;

    private Mover _mover;
    private Camera _camera;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (_target != null)        
            _target.TryGetComponent(out _mover);        
    }

    private void OnEnable()
    {
        if (_mover != null)
            _mover.Flipped += InvertXOffset;
    }

    private void FixedUpdate()
    {
        if (_target == null)
            return;

        Vector3 targetPosition = new Vector3(_target.position.x + _offset.x, _target.position.y + _offset.y, _zOffset);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 1f / _smoothSpeed);
    }
    private void OnDisable()
    {
        if (_mover != null)
            _mover.Flipped -= InvertXOffset;
    }

    public void SetTarget(Transform target)
    {
        if (_mover != null)
            _mover.Flipped -= InvertXOffset;

        _target = target;

        if (_target != null)
        {
            _target.TryGetComponent(out _mover);

            if (_mover != null)
                _mover.Flipped += InvertXOffset;
        }
        else
        {
            _mover = null;
        }

        _velocity = Vector3.zero;
    }

    private void InvertXOffset()
    {
        _offset.x = -_offset.x;
    }
}