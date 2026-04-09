using Spawners;

public class EnemySpawner : OneTimeSpawner<Enemy, EnemySpawnPoint>
{
    protected override void InitializeItem(Enemy enemy, EnemySpawnPoint spawnPoint)
    {
        base.InitializeItem(enemy, spawnPoint);
        enemy.Init(spawnPoint.PatrolPoints);
        enemy.Falled += HandleEnemyFall;
    }

    private void HandleEnemyFall(Enemy enemy)
    {
        enemy.Falled -= HandleEnemyFall;
        ReleaseToPool(enemy);
    }
}