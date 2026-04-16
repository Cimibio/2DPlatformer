using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(HeartAnimator))]
public class Heart : Collectable, IPickupable
{
    [SerializeField] private int _healAmount = 10;

    public int HealAmount => _healAmount;
}
