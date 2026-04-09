using Spawners;

public class EnemySpawner : OneTimeSpawner<Enemy, EnemySpawnPoint>
{
    protected override void InitializeItem(Enemy enemy, EnemySpawnPoint spawnPoint)
    {
        enemy.transform.position = spawnPoint.transform.position;
        enemy.Init(spawnPoint.PatrolPoints);
        enemy.Falled += HandleEnemyFall;
    }

    private void HandleEnemyFall(Enemy enemy)
    {
        enemy.Falled -= HandleEnemyFall;
        ReleaseToPool(enemy);
    }
}