using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TargetDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float _checkInterval = 0.1f;
    [SerializeField] private float _targetLostDelay = 3f;
    [SerializeField] private bool _debugMode = true;

    private CircleCollider2D _visionCollider;
    private Transform _detectedTarget;
    private float _checkTimer;
    private bool _hasLineOfSight;
    private Vector3 _lastKnownPosition;
    private float _lostTimer;
    private bool _isTargetInVisionZone;

    public bool HasTarget => _detectedTarget != null;
    public bool HasLineOfSight => _hasLineOfSight;
    public Transform Target => _detectedTarget;
    public Vector3 LastKnownPosition => _lastKnownPosition;

    private void Awake()
    {
        _visionCollider = GetComponent<CircleCollider2D>();
        _visionCollider.isTrigger = true;
    }

    private void Update()
    {
        if (_detectedTarget == null)
            return;

        _checkTimer += Time.deltaTime;

        if (_checkTimer >= _checkInterval)
        {
            _checkTimer = 0;
            UpdateLineOfSight();
        }

        if (!_isTargetInVisionZone && !_hasLineOfSight)
        {
            _lostTimer += Time.deltaTime;

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Target lost timer: {_lostTimer:F1}/{_targetLostDelay}s");

            if (_lostTimer >= _targetLostDelay)
            {
                if (_debugMode)
                    Debug.Log($"[{gameObject.name}] Target completely lost");

                ClearTarget();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsTargetLayer(other))
        {
            _detectedTarget = other.transform;
            _isTargetInVisionZone = true;
            _lostTimer = 0f;
            _lastKnownPosition = _detectedTarget.position;
            UpdateLineOfSight();

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Target entered vision zone");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsTargetLayer(other) && _detectedTarget == other.transform)
        {
            _isTargetInVisionZone = false;
            _hasLineOfSight = false;
            _lostTimer = 0f;

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Target left vision zone, keeping for search");
        }
    }

    private void UpdateLineOfSight()
    {
        if (_detectedTarget == null)
        {
            _hasLineOfSight = false;
            return;
        }

        Vector2 direction = _detectedTarget.position - transform.position;
        float distance = direction.magnitude;
        bool previousLineOfSight = _hasLineOfSight;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, _obstacleLayer);

        _hasLineOfSight = hit.collider == null;

        if (_hasLineOfSight)
        {
            _lastKnownPosition = _detectedTarget.position;
            _lostTimer = 0f;
        }

        if (_hasLineOfSight && !previousLineOfSight)
        {
            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Line of sight gained");
        }
        else if (!_hasLineOfSight && previousLineOfSight)
        {
            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Line of sight lost");
        }

        if (_debugMode)
            Debug.DrawRay(transform.position, direction, _hasLineOfSight ? Color.green : Color.red, 0.1f);
    }

    private void ClearTarget()
    {
        _detectedTarget = null;
        _hasLineOfSight = false;
        _isTargetInVisionZone = false;
        _lostTimer = 0f;
    }

    private bool IsTargetLayer(Collider2D other)
    {
        return (_targetLayer.value & (1 << other.gameObject.layer)) != 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (_visionCollider != null)
        {
            Gizmos.color = HasTarget && HasLineOfSight ? Color.red :
                          HasTarget && !HasLineOfSight ? Color.yellow :
                          Color.gray;
            Gizmos.DrawWireSphere(transform.position, _visionCollider.radius);
        }
    }
}