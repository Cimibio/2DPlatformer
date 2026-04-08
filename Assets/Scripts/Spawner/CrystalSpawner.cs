using Spawners;
using UnityEngine;

public class CrystalSpawner : OneTimeSpawner<Crystal>
{
    [SerializeField] private CrystalSpawnPoints _spawnPoints;

    private CrystalSpawnPoint[] _points;

    protected override void SpawnAll()
    {
        if (_spawnPoints == null || _spawnPoints.Points.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned to CrystalSpawner!");
            return;
        }

        _points = new CrystalSpawnPoint[_spawnPoints.Points.Count];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = _spawnPoints.Points[i];

        Debug.Log($"Crystal Spawner Started, created {_points.Length} spawn points");

        for (int i = 0; i < _points.Length; i++)
        {
            Crystal crystal = Pool.Get();
            crystal.Init();
            crystal.transform.position = _points[i].transform.position;
            crystal.Collected += HandleCrystalCollected;
            Debug.Log($"Crystal spawned at {_points[i].name}");
        }
    }

    private void HandleCrystalCollected(Crystal crystal)
    {
        Debug.Log($"Crystal {crystal.name} collected");
        crystal.Collected -= HandleCrystalCollected;
        ReleaseToPool(crystal);
    }
}