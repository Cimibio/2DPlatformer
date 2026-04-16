using Spawners;

public class CrystalSpawner : PeriodicSpawner<Crystal, CollectableSpawnPoint>
{
    protected override void InitializeItem(Crystal crystal, CollectableSpawnPoint spawnPoint)
    {
        base.InitializeItem(crystal, spawnPoint);
        crystal.Init();
    }
}