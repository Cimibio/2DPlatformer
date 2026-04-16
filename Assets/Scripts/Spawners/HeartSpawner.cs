using Spawners;

public class HeartSpawner : PeriodicSpawner<Heart, CollectableSpawnPoint>
{
    protected override void InitializeItem(Heart heart, CollectableSpawnPoint spawnPoint)
    {
        base.InitializeItem(heart, spawnPoint);
        heart.Init();
    }
}
