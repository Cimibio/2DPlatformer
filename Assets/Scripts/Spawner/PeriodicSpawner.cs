using System.Collections;
using UnityEngine;

namespace Spawners
{
    public abstract class PeriodicSpawner<TItem, TPoint> : Spawner<TItem>
        where TItem : MonoBehaviour
        where TPoint : class, ISpawnPoint
    {
        [SerializeField] private MonoBehaviour _spawnPointsContainer;
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private bool _randomPoint = false;

        private ISpawnPointsContainer<TPoint> _container;
        private Coroutine _spawnCoroutine;
        private WaitForSeconds _waitInterval;
        private bool _isSpawning = false;

        protected override void Awake()
        {
            base.Awake();
            _waitInterval = new WaitForSeconds(_spawnInterval);

            if (_spawnPointsContainer == null)
            {
                Debug.LogError($"Spawn points container not assigned to {gameObject.name}");
                return;
            }

            _container = _spawnPointsContainer as ISpawnPointsContainer<TPoint>;

            if (_container == null)
            {
                Debug.LogError($"{_spawnPointsContainer.name} does not implement ISpawnPointsContainer<{typeof(TPoint).Name}>");
            }
        }

        protected override void Start()
        {
            if (_spawnOnStart)
            {
                StartSpawning();
            }
        }

        public void StartSpawning()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            _isSpawning = true;
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        public void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                _isSpawning = false;
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (_isSpawning)
            {
                yield return _waitInterval;
                SpawnOne();
            }
        }

        protected void SpawnOne()
        {
            if (_container == null || _container.Points.Count == 0)
            {
                Debug.LogWarning($"No spawn points for {gameObject.name}");
                return;
            }

            TPoint spawnPoint;

            if (_randomPoint)
            {
                int randomIndex = Random.Range(0, _container.Points.Count);
                spawnPoint = _container.Points[randomIndex];
            }
            else
            {
                spawnPoint = GetNextPoint();
            }

            TItem item = GetFromPool();
            InitializeItem(item, spawnPoint);
        }

        protected virtual TPoint GetNextPoint()
        {
            return _container.Points[0];
        }

        protected abstract void InitializeItem(TItem item, TPoint spawnPoint);
    }
}
