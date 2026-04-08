using Spawners;
using UnityEngine;

public class EnemySpawner : OneTimeSpawner<Enemy>
{
    [SerializeField] private EnemySpawnPoints _spawnPoints;

    private EnemySpawnPoint[] _points;

    protected override void SpawnAll()
    {
        if (_spawnPoints == null || _spawnPoints.Points.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned to EnemySpawner!");
            return;
        }

        _points = new EnemySpawnPoint[_spawnPoints.Points.Count];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = _spawnPoints.Points[i];

        Debug.Log($"Enemy Spawner Started, created {_points.Length} spawn points");

        for (int i = 0; i < _points.Length; i++)
        {
            Enemy enemy = Pool.Get();
            enemy.transform.position = _points[i].transform.position;
            enemy.Init(_points[i].PatrolPoints);
            enemy.Falled += HandleEnemyFall;
            Debug.Log($"Enemy spawned at {_points[i].name}");
        }
    }

    private void HandleEnemyFall(Enemy enemy)
    {
        enemy.Falled -= HandleEnemyFall;
        ReleaseToPool(enemy);
    }
}