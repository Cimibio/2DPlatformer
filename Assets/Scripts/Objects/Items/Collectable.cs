using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(CollectableAnimator))]
public abstract class Collectable : MonoBehaviour, IPickupable
{
    protected CollideDetector _collideDetector;
    protected CollectableAnimator _animator;

    public event Action<IPickupable> PickedUp;

    protected virtual void Awake()
    {
        _collideDetector = GetComponent<CollideDetector>();
        _animator = GetComponent<CollectableAnimator>();
    }

    protected virtual void OnEnable()
    {
        _collideDetector.Collided += PickUp;

        if (_animator != null)
            _animator.CollectionAnimationCompleted += NotifyItemCollection;
    }

    protected virtual void OnDisable()
    {
        _collideDetector.Collided -= PickUp;

        if (_animator != null)
            _animator.CollectionAnimationCompleted -= NotifyItemCollection;
    }

    public virtual void Init()
    {
        if (_animator != null)
            _animator.ResetToIdle();
    }

    protected virtual void NotifyItemCollection()
    {
        PickedUp?.Invoke(this);
    }

    public virtual void PickUp()
    {
        if (_animator != null)
            _animator.PlayDisappearAnimation();
    }
}