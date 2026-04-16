using UnityEngine;

[RequireComponent(typeof(CrystalAnimator))]
public class Crystal : Collectable
{
    [SerializeField] private int _scoreValue = 1;

    public int ScoreValue => _scoreValue;

    protected override void ApplyEffect(Player player)
    {
        player.AddScore(_scoreValue);
        Debug.Log($"[Crystal] Added {_scoreValue} score");
    }
}