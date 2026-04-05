using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector))]
public class Crystal : MonoBehaviour
{
    private CollideDetector _collideDetector;

    public event Action<Crystal> Collected;

    private void Awake()
    {
        _collideDetector = GetComponent<CollideDetector>();        
    }

    private void OnEnable()
    {
        _collideDetector.Collided += Collect;
    }

    private void OnDisable()
    {
        _collideDetector.Collided -= Collect;
    }

    private void Collect()
    {
        Debug.Log($"Crystal detect collision with player");
        Collected?.Invoke(this);
    }
}
