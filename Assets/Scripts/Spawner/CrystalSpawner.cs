using Spawners;

public class CrystalSpawner : PeriodicSpawner<Crystal, CrystalSpawnPoint>
{
    protected override void InitializeItem(Crystal crystal, CrystalSpawnPoint spawnPoint)
    {
        base.InitializeItem(crystal, spawnPoint);
        crystal.Init();
    }
}