using System.Collections;
using UnityEngine;

namespace Spawners
{
    public abstract class PeriodicSpawner<TItem, TPoint> : OneTimeSpawner<TItem, TPoint>
        where TItem : MonoBehaviour
        where TPoint : class, ISpawnPoint
    {
        [SerializeField] private float _respawnInterval = 5f;
        [SerializeField] private bool _respawnOnPickup = true;
        [SerializeField] private bool _randomPointOnRespawn = false;

        private WaitForSeconds _respawnWaitInterval;
        private bool _isRespawning = false;

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

            // Подписываемся на событие "предмет использован/подобран"
            if (item is IPickupable pickupable)
            {
                pickupable.PickedUp += HandleItemPickedUp;
            }
        }

        protected void SpawnOne()
        {
            TPoint spawnPoint = _randomPointOnRespawn
                ? GetRandomSpawnPoint()
                : GetNextSpawnPoint();

            TItem item = GetFromPool();
            InitializeItem(item, spawnPoint);
        }

        protected virtual TPoint GetNextSpawnPoint()
        {
            return _container?.Points[0];
        }

        public void StartRespawning()
        {
            _isRespawning = true;
        }

        public void StopRespawning()
        {
            _isRespawning = false;
        }


        private void HandleItemPickedUp(IPickupable item)
        {
            if (!_respawnOnPickup) return;

            var monoItem = item as TItem;
            if (monoItem == null) return;

            if (item is IPickupable pickupable)
            {
                pickupable.PickedUp -= HandleItemPickedUp;
            }

            ReleaseToPool(monoItem);

            StartCoroutine(RespawnCoroutine());
        }
        private IEnumerator RespawnCoroutine()
        {
            _isRespawning = true;
            yield return _respawnWaitInterval;

            if (_isRespawning)
            {
                SpawnOne();
            }

            _isRespawning = false;
        }

        private TPoint GetRandomSpawnPoint()
        {
            if (_container == null || _container.Points.Count == 0)
                return null;

            int randomIndex = Random.Range(0, _container.Points.Count);
            return _container.Points[randomIndex];
        }
    }
}