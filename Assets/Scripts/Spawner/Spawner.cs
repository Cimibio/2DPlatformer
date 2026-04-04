using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

namespace Spawners
{
    public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T _prefab;
        [SerializeField] private float _repeatRate = 10f;
        [SerializeField] private int _poolCapacity = 20;
        [SerializeField] private int _poolMaxSize = 20;

        protected ObjectPool<T> Pool;
        private bool _isSpawning = true;
        private Coroutine _spawnCoroutine;

        private void Awake()
        {
            Pool = new ObjectPool<T>(
                createFunc: () => Instantiate(_prefab),
                actionOnGet: Spawn,
                actionOnRelease: Despawn,
                actionOnDestroy: (obj) => Destroy(obj.gameObject),
                collectionCheck: true,
                defaultCapacity: _poolCapacity,
                maxSize: _poolMaxSize
            );
            Debug.Log("Spawner awaked, Pool created");
        }

        protected virtual void Start()
        {
            Debug.Log("Spawner started");
            StartSpawning();
        }

        private void OnDisable()
        {
            StopSpawning();
        }

        protected T GetFromPool()
        {
            return Pool.Get();
        }

        protected void ReleaseToPool(T obj)
        {
            Pool.Release(obj);
        }
        protected virtual void Despawn(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        protected virtual void Spawn(T obj)
        {
            obj.gameObject.SetActive(true);
        }
        protected void StopSpawning()
        {
            Debug.Log("Stoping spawning");
            if (_spawnCoroutine != null)
            {
                _isSpawning = false;
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private void StartSpawning()
        {
            Debug.Log("Starting spawning");
            if (_spawnCoroutine == null)
                _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }


        private IEnumerator SpawnRoutine()
        {
            Debug.Log("Spawn coroutine started");
            var wait = new WaitForSeconds(_repeatRate);

            while (_isSpawning)
            {
                GetFromPool();
                yield return wait;
            }
        }
    }
}
