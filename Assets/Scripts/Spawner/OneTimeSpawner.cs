using UnityEngine;

namespace Spawners
{
    public abstract class OneTimeSpawner<T> : Spawner<T> where T : MonoBehaviour
    {
        protected sealed override void Start()
        {
            base.Start();
            SpawnAll();
        }

        protected abstract void SpawnAll();
    }
}