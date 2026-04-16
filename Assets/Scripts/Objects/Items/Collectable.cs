using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(CollectableAnimator))]
public abstract class Collectable : MonoBehaviour, IPickupable
{
    protected CollideDetector _collideDetector;
    protected CollectableAnimator _animator;

    private bool _isCollected = false;

    public event Action<IPickupable> PickedUp;

    protected virtual void Awake()
    {
        _collideDetector = GetComponent<CollideDetector>();
        _animator = GetComponent<CollectableAnimator>();
    }

    protected virtual void OnEnable()
    {
        _isCollected = false;
        _collideDetector.enabled = true;
        _collideDetector.Collided += Collect;

        if (_animator != null)
            _animator.CollectionAnimationCompleted += NotifyItemCollection;
    }

    protected virtual void OnDisable()
    {
        _collideDetector.Collided -= Collect;

        if (_animator != null)
            _animator.CollectionAnimationCompleted -= NotifyItemCollection;
    }

    protected virtual void Collect(Collider2D other)
    {
        if (_isCollected)
            return;

        if (other.TryGetComponent(out Player player))
        {
            if (!player.IsAlive)
                return;

            _isCollected = true;
            _collideDetector.enabled = false;

            ApplyEffect(player);
            PickUp();
        }
    }

    protected abstract void ApplyEffect(Player player);

    public virtual void Init()
    {
        _isCollected = false;
        _collideDetector.enabled = true;

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