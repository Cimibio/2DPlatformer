using UnityEngine;
using UnityEngine.Pool;

namespace Spawners
{
    public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T _prefab;
        [SerializeField] private int _poolCapacity = 20;
        [SerializeField] private int _poolMaxSize = 20;

        protected ObjectPool<T> Pool;

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
    }
}
