using Spawners;
using UnityEngine;

public class EnemySpawner : Spawner<Enemy>
{
    [SerializeField] private EnemySpawnPoints _spawnPoints;

    private EnemySpawnPoint[] _points;

    protected override void Start()
    {
        if (_spawnPoints == null || _spawnPoints.Points.Count == 0)
            return;

        _points = new EnemySpawnPoint[_spawnPoints.Points.Count];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = _spawnPoints.Points[i];

        Debug.Log($"Enemy Spawner Started, created {_points.Length} spawn points");
        SpawnAllEnemies();
    }

    private void SpawnAllEnemies()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            Enemy enemy = Pool.Get();

            enemy.transform.position = _points[i].transform.position;
            enemy.Init(_points[i].PatrolPoints);
            enemy.Falled += Fall;

            Debug.Log($"Enemy spawned at {_points[i].name}");
        }
    }

    private void Fall(Enemy enemy)
    {
        enemy.Falled -= Fall;
        ReleaseToPool(enemy);
    }
}

