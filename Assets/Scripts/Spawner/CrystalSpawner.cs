using Spawners;
using UnityEngine;

public class CrystalSpawner : Spawner<Crystal>
{
    [SerializeField] private CrystalSpawnPoints _spawnPoints;

    private CrystalSpawnPoint[] _points;

    protected override void Start()
    {
        if (_spawnPoints == null || _spawnPoints.Points.Count == 0)
            return;

        _points = new CrystalSpawnPoint[_spawnPoints.Points.Count];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = _spawnPoints.Points[i];

        Debug.Log($"Crystal Spawner Started, created {_points.Length} spawn points");
        SpawnAllCrystal();
    }

    protected override void Spawn(Crystal crystal)
    {
        base.Spawn(crystal);
    }

    protected override void Despawn(Crystal crystal)
    {
        crystal.Collected -= Despawn;
        ReleaseToPool(crystal);
    }

    private void SpawnAllCrystal()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            Crystal crystal = Pool.Get();

            crystal.transform.position = _points[i].transform.position;
            crystal.Collected += Despawn;

            Debug.Log($"Enemy spawned at {_points[i].name}");
        }
    }
}
