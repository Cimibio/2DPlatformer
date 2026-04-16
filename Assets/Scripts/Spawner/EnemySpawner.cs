using Spawners;

public class EnemySpawner : OneTimeSpawner<Enemy, EnemySpawnPoint>
{
    protected override void InitializeItem(Enemy enemy, EnemySpawnPoint spawnPoint)
    {
        base.InitializeItem(enemy, spawnPoint);
        enemy.Init(spawnPoint.PatrolPoints);
        enemy.Falled += HandleEnemyDeath;
        enemy.Died += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemy.Falled -= HandleEnemyDeath;
        enemy.Died -= HandleEnemyDeath;
        ReleaseToPool(enemy);
    }
}