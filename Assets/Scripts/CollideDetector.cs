using System;
using UnityEngine;

public class CollideDetector : MonoBehaviour
{
    private Collider2D _collider;

    public event Action Collided;

    private void Start()
    {
        if (TryGetComponent(out Collider2D collider))
        {
            _collider = collider;
            _collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Player>(out _))
            Collided?.Invoke();

        Debug.Log($"Detector detect collision with {collider.gameObject.name}");
    }
}
