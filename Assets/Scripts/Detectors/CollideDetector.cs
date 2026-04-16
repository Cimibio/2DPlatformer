using System;
using UnityEngine;

public class CollideDetector : MonoBehaviour
{
    [SerializeField] protected LayerMask _targetLayer;
    [SerializeField] protected bool _debugMode = true;

    protected int _targetLayerValue;

    public event Action<Collider2D> Collided;

    protected virtual void Awake()
    {
        _targetLayerValue = _targetLayer.value;

        if (!TryGetComponent(out Collider2D collider))
        {
            Debug.LogError($"[{gameObject.name}] CollideDetector requires a Collider2D component!");
            enabled = false;
            return;
        }

        SetupCollider(collider);
    }

    protected virtual void SetupCollider(Collider2D collider)
    {
        collider.isTrigger = true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (IsTargetLayer(other))
        {
            Collided?.Invoke(other);

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Detected collision with {other.gameObject.name}");

            HandleCollision(other);
        }
    }

    protected virtual bool IsTargetLayer(Collider2D other)
    {
        return (_targetLayerValue & (1 << other.gameObject.layer)) != 0;
    }

    protected virtual void HandleCollision(Collider2D other)
    {    }
}