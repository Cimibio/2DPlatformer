using Spawners;
using UnityEngine;

public class EnemySpawner : Spawner<Enemy>
{
    [SerializeField] private SpawnPoint _spawnPoint;
    [SerializeField] private Transform _patrolPoints;

    protected override void Spawn(Enemy enemy)
    {
        base.Spawn(enemy);
        //Vector3 spawnPointPosition = _spawnPoint.GetTransform.position;
        enemy.transform.position = _spawnPoint.GetTransform.position;

        enemy.Init(_patrolPoints);
        //enemy.Falled += OnEnemyFall;
    }

    //private void OnEnemyFall(Enemy enemy)
    //{
    //    //enemy.Falled -= OnEnemyFall;
    //    ReleaseToPool(enemy);
    //}
}

