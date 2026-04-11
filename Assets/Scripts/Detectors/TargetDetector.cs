using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TargetDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private bool _debugMode = true;

    private CircleCollider2D _visionCollider;
    private Transform _detectedTarget;

    public event Action<Transform> TargetEntered;
    public event Action TargetExited;

    public bool HasTarget => _detectedTarget != null;
    public Transform Target => _detectedTarget;

    private void Awake()
    {
        _visionCollider = GetComponent<CircleCollider2D>();
        _visionCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsTargetLayer(other))
        {
            _detectedTarget = other.transform;

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Target detected: {other.name}");

            TargetEntered?.Invoke(_detectedTarget);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsTargetLayer(other) && _detectedTarget == other.transform)
        {
            _detectedTarget = null;

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Target lost: {other.name}");

            TargetExited?.Invoke();
        }
    }

    private bool IsTargetLayer(Collider2D other)
    {
        return (_targetLayer.value & (1 << other.gameObject.layer)) != 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (_visionCollider != null)
        {
            Gizmos.color = HasTarget ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _visionCollider.radius);
        }
    }
}