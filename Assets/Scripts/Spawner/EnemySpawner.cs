using Spawners;
using UnityEngine;

public class EnemySpawner : Spawner<Enemy>
{
    [SerializeField] private SpawnPoints _spawnPoints;

    private SpawnPoint[] _points;

    protected override void Start()
    {
        if (_spawnPoints == null || _spawnPoints.Points.Count == 0)
            return;

        _points = new SpawnPoint[_spawnPoints.Points.Count];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = _spawnPoints.Points[i];

        Debug.Log($"Enemy Spawner Started, created {_points.Length} spawn points");
        SpawnAllEnemies();
    }

    protected override void Spawn(Enemy enemy)
    {
        //for (int i = 0; i < _points.Length; i++)
        //{
        //    base.Spawn(enemy);
        //    enemy.transform.position = _points[i].transform.position;

        //    enemy.Init(_points[i].PatrolPoints);

        //    Debug.Log($"Enemy spawned at Spawn Point {i}");
        //    enemy.Falled += OnEnemyFall;
        //}

        base.Spawn(enemy);
    }

    private void SpawnAllEnemies()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            Enemy enemy = Pool.Get();

            enemy.transform.position = _points[i].transform.position;
            enemy.Init(_points[i].PatrolPoints);
            enemy.Falled += OnEnemyFall;

            Debug.Log($"Enemy spawned at {_points[i].name}");
        }
    }

    private void OnEnemyFall(Enemy enemy)
    {
        enemy.Falled -= OnEnemyFall;
        ReleaseToPool(enemy);
    }
}

