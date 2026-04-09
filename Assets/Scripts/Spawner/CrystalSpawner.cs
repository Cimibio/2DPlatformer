using Spawners;

public class CrystalSpawner : OneTimeSpawner<Crystal, CrystalSpawnPoint>
{
    protected override void InitializeItem(Crystal crystal, CrystalSpawnPoint spawnPoint)
    {
        crystal.transform.position = spawnPoint.transform.position;
        crystal.Init();
        crystal.Collected += HandleCrystalCollected;
    }

    private void HandleCrystalCollected(Crystal crystal)
    {
        crystal.Collected -= HandleCrystalCollected;
        ReleaseToPool(crystal);
    }
}