using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(CrystalAnimator))]
public class Crystal : MonoBehaviour
{
    private CollideDetector _collideDetector;
    private CrystalAnimator _crystalAnimator;
    //private CircleCollider2D _collider;
    private bool _isCollected = false;

    public event Action<Crystal> Collected;

    private void Awake()
    {
        _collideDetector = GetComponent<CollideDetector>(); 
        _crystalAnimator = GetComponent<CrystalAnimator>();
    }

    private void OnEnable()
    {
        _collideDetector.Collided += Collect;
        _crystalAnimator.CollectionAnimationCompleted += OnCollectionAnimationCompleted;
    }

    private void OnDisable()
    {
        _collideDetector.Collided -= Collect; 
        _crystalAnimator.CollectionAnimationCompleted -= OnCollectionAnimationCompleted;
    }

    private void OnCollectionAnimationCompleted()
    {
        Collected?.Invoke(this);
    }

    private void Collect()
    {
        _isCollected = true;
        _crystalAnimator.PlayDisapearAnimation();
    }

    public void ResetState()
    {
        _isCollected = false;
        _crystalAnimator.ResetToIdle();
    }
}
