using UnityEngine;

[RequireComponent(typeof(HeartAnimator))]
public class Heart : Collectable
{
    [SerializeField] private int _healAmount = 10;

    public int HealAmount => _healAmount;

    protected override void ApplyEffect(Player player)
    {
        player.Heal(_healAmount);
        Debug.Log($"[Heart] Healed player for {_healAmount}");
    }
}