using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(CrystalAnimator))]
public class Crystal : MonoBehaviour, IPickupable
{
    private CollideDetector _collideDetector;
    private CrystalAnimator _crystalAnimator;

    public event Action<IPickupable> PickedUp;

    private void Awake()
    {
        _collideDetector = GetComponent<CollideDetector>(); 
        _crystalAnimator = GetComponent<CrystalAnimator>();
    }

    private void OnEnable()
    {
        _collideDetector.Collided += PickUp;
        _crystalAnimator.CollectionAnimationCompleted += NotifyItemCollection;
    }

    private void OnDisable()
    {
        _collideDetector.Collided -= PickUp; 
        _crystalAnimator.CollectionAnimationCompleted -= NotifyItemCollection;
    }

    public void Init()
    {
        _crystalAnimator.ResetToIdle();
    }

    private void NotifyItemCollection()
    {
        PickedUp?.Invoke(this);
    }

    public void PickUp()
    {
        _crystalAnimator.PlayDisapearAnimation();
    }
}
