using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCollector : MonoBehaviour
{
    [SerializeField] private LayerMask _collectableLayer;
    [SerializeField] private bool _debugMode = true;

    private Player _player;
    private int _collectableLayerValue;
    private CircleCollider2D _collectTrigger;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _collectableLayerValue = _collectableLayer.value;

        _collectTrigger = GetComponent<CircleCollider2D>();
        _collectTrigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_collectableLayerValue & (1 << other.gameObject.layer)) == 0)
            return;

        if (other.TryGetComponent(out Collectable collectable))
        {
            Collect(collectable);
        }
    }

    private void Collect(Collectable collectable)
    {
        if (!_player.IsAlive)
            return;

        switch (collectable)
        {
            case Heart heart:
                _player.Heal(heart.HealAmount);
                break;

            case Crystal crystal:
                _player.AddScore(crystal.ScoreValue);
                break;
        }

        if (_debugMode)
            Debug.Log($"[Player] Collected {collectable.GetType().Name}");

        collectable.PickUp();
    }
}