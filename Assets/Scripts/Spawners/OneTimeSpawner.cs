using UnityEngine;

namespace Spawners
{
    public abstract class OneTimeSpawner<TItem, TPoint> : Spawner<TItem>
        where TItem : MonoBehaviour
        where TPoint : class, ISpawnPoint
    {
        [SerializeField] private MonoBehaviour _spawnPointsContainer;

        protected ISpawnPointsContainer<TPoint> _container;

        protected override void Awake()
        {
            base.Awake();

            if (_spawnPointsContainer == null)
            {
                Debug.LogError($"Spawn points container not assigned to {gameObject.name}");
                return;
            }

            _container = _spawnPointsContainer as ISpawnPointsContainer<TPoint>;

            if (_container == null)
            {
                Debug.LogError($"{_spawnPointsContainer.name} does not implement ISpawnPointsContainer<{typeof(TPoint).Name}>. " +
                    $"Actual type: {_spawnPointsContainer.GetType().Name}");
                return;
            }

            Debug.Log($"Container found: {_container.Points.Count} spawn points");
        }

        protected override void Start()
        {
            SpawnAll();
        }

        protected void SpawnAll()
        {
            if (_container == null)
            {
                Debug.LogError($"[{gameObject.name}] Container is null! Check if OnAwake was called and container assigned.");
                return;
            }

            if (_container.Points.Count == 0)
            {
                Debug.LogWarning($"[{gameObject.name}] Container has {_container.Points.Count} spawn points. " +
                    $"Make sure points are assigned in the Inspector.");
                return;
            }

            Debug.Log($"[{gameObject.name}] Spawning {_container.Points.Count} items");

            foreach (var spawnPoint in _container.Points)
            {
                TItem item = GetFromPool();
                InitializeItem(item, spawnPoint);
            }
        }

        protected virtual void InitializeItem(TItem item, TPoint spawnPoint)
        {
            item.transform.position = spawnPoint.transform.position;
        }
    }
}