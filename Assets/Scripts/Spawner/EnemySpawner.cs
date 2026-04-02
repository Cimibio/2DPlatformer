using Spawners;
using UnityEngine;

public class EnemySpawner : Spawner<Enemy>
{
    [SerializeField] private SpawnPoint _spawnPoint;

    protected override void Spawn(Enemy enemy)
    {
        Vector3 spawnPointPosition = _spawnPoint.GetTransform.position;

        base.Spawn(enemy);

        //enemy.Falled += OnEnemyFall;
    }

    //private void OnEnemyFall(Enemy enemy)
    //{
    //    //enemy.Falled -= OnEnemyFall;
    //    ReleaseToPool(enemy);
    //}
}

