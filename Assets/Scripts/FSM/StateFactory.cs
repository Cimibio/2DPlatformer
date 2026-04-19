public class StateFactory
{
    protected readonly SlimeEnemyBehavior _behavior;

    public StateFactory(SlimeEnemyBehavior behavior)
    {
        _behavior = behavior;
    }

    public EnemyPatrolState CreatePatrolState() => new EnemyPatrolState(_behavior);
    public EnemyCombatState CreateCombatState() => new EnemyCombatState(_behavior);
}