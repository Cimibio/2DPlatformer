using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(CrystalAnimator))]
public class Crystal : MonoBehaviour
{
    private CollideDetector _collideDetector;
    private CrystalAnimator _crystalAnimator;

    public event Action<Crystal> Collected;

    private void Awake()
    {
        _collideDetector = GetComponent<CollideDetector>(); 
        _crystalAnimator = GetComponent<CrystalAnimator>();
    }

    private void OnEnable()
    {
        _collideDetector.Collided += Collect;
        _crystalAnimator.CollectionAnimationCompleted += NotifyItemCollection;
    }

    private void OnDisable()
    {
        _collideDetector.Collided -= Collect; 
        _crystalAnimator.CollectionAnimationCompleted -= NotifyItemCollection;
    }

    public void Init()
    {
        _crystalAnimator.ResetToIdle();
    }

    private void NotifyItemCollection()
    {
        Collected?.Invoke(this);
    }

    private void Collect()
    {
        _crystalAnimator.PlayDisapearAnimation();
    }
}
