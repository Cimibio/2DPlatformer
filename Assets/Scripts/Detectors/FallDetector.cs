using UnityEngine;
using System;

public class FallDetector : MonoBehaviour
{
    [SerializeField] private float _fallThreshold = -10f;

    public event Action Falled;

    private void Update()
    {
        if (transform.position.y < _fallThreshold)
            Falled?.Invoke();
    }
}