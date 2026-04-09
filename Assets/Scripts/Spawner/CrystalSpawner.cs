using Spawners;

public class CrystalSpawner : OneTimeSpawner<Crystal, CrystalSpawnPoint>
{
    protected override void InitializeItem(Crystal crystal, CrystalSpawnPoint spawnPoint)
    {
        base.InitializeItem(crystal, spawnPoint);
        crystal.Init();
        crystal.Collected += HandleCrystalCollected;
    }

    private void HandleCrystalCollected(Crystal crystal)
    {
        crystal.Collected -= HandleCrystalCollected;
        ReleaseToPool(crystal);
    }
}