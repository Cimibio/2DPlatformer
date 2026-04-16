using System;
using UnityEngine;

public class CollideDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private bool _debugMode = true;

    private Collider2D _collider;
    private int _targetLayerValue;

    public event Action Collided;

    private void Awake()
    {
        _targetLayerValue = _targetLayer.value;

        if (TryGetComponent(out Collider2D collider))
        {
            _collider = collider;
            _collider.isTrigger = true;
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] CollideDetector requires a Collider2D component!");
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_targetLayerValue & (1 << other.gameObject.layer)) != 0)
        {
            Collided?.Invoke();

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Detected collision with {other.gameObject.name}");
        }
    }
}