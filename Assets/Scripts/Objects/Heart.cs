using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(HeartAnimator))]
public class Heart : MonoBehaviour, IPickupable
{
    private CollideDetector _collideDetector;
    private HeartAnimator _animator;

    public event Action<IPickupable> PickedUp;

    private void Awake()
    {
        _collideDetector = GetComponent<CollideDetector>();
        _animator = GetComponent<HeartAnimator>();
    }

    private void OnEnable()
    {
        _collideDetector.Collided += PickUp;
        _animator.CollectionAnimationCompleted += NotifyItemCollection;
    }

    private void OnDisable()
    {
        _collideDetector.Collided -= PickUp;
        _animator.CollectionAnimationCompleted -= NotifyItemCollection;
    }

    public void Init()
    {
        _animator.ResetToIdle();
    }

    private void NotifyItemCollection()
    {
        PickedUp?.Invoke(this);
    }

    public void PickUp()
    {
        _animator.PlayDisapearAnimation();
    }
}
