using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawners
{
    public abstract class PeriodicSpawner<TItem, TPoint> : OneTimeSpawner<TItem, TPoint>
        where TItem : MonoBehaviour
        where TPoint : class, ISpawnPoint
    {
        [SerializeField] private float _respawnInterval = 5f;
        [SerializeField] private bool _respawnOnPickup = true;

        private WaitForSeconds _respawnWaitInterval;
        private Dictionary<TItem, TPoint> _itemsToSpawn = new Dictionary<TItem, TPoint>();

        protected override void Awake()
        {
            base.Awake();
            _respawnWaitInterval = new WaitForSeconds(_respawnInterval);
        }

        protected override void Start()
        {
            base.Start();
        }
        protected virtual void OnDestroy()
        {
            StopAllCoroutines();
        }

        protected override void InitializeItem(TItem item, TPoint spawnPoint)
        {
            base.InitializeItem(item, spawnPoint);

            _itemsToSpawn[item] = spawnPoint;

            if (item is IPickupable pickupable)
                pickupable.PickedUp += HandleItemPickedUp;
        }

        protected void SpawnAtPoint(TPoint spawnPoint)
        {
            if (spawnPoint == null)
            {
                Debug.LogWarning($"Cannot spawn at null point for {gameObject.name}");
                return;
            }

            TItem item = GetFromPool();
            InitializeItem(item, spawnPoint);
        }

        private void HandleItemPickedUp(IPickupable item)
        {
            if (!_respawnOnPickup) 
                return;

            var monoItem = item as TItem;

            if (monoItem == null) 
                return;

            if (!_itemsToSpawn.TryGetValue(monoItem, out TPoint originalSpawnPoint))
            {
                Debug.LogWarning($"Cannot find spawn point for {monoItem.name}");
                return;
            }

            if (item is IPickupable pickupable)            
                pickupable.PickedUp -= HandleItemPickedUp;

            _itemsToSpawn.Remove(monoItem);

            ReleaseToPool(monoItem);

            StartCoroutine(RespawnAtPointCoroutine(originalSpawnPoint));
        }

        private IEnumerator RespawnAtPointCoroutine(TPoint spawnPoint)
        {
            yield return _respawnWaitInterval;

            SpawnAtPoint(spawnPoint);
        }
    }
}