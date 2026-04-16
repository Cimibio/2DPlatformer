using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(CollideDetector), typeof(CrystalAnimator))]
public class Crystal : Collectable, IPickupable
{
    [SerializeField] private int _scoreValue = 1;

    public int ScoreValue => _scoreValue;
}
