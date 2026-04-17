using static SlimeEnemyBehavior;

public class StateFactory
{
    private readonly SlimeEnemyBehavior _behavior;

    public StateFactory(SlimeEnemyBehavior behavior)
    {
        _behavior = behavior;
    }

    public EnemyState CreateState(StateType stateType)
    {
        return stateType switch
        {
            StateType.Patrol => CreatePatrolState(),
            StateType.Chase => CreateChaseState(),
            StateType.Attack => CreateAttackState(),
            StateType.Search => CreateSearchState(),
            StateType.Idle => CreateIdleState(),
            _ => CreatePatrolState()
        };
    }

    public EnemyPatrolState CreatePatrolState() => new EnemyPatrolState(_behavior);
    public EnemyChaseState CreateChaseState() => new EnemyChaseState(_behavior);
    public EnemyAttackState CreateAttackState() => new EnemyAttackState(_behavior);
    public EnemySearchState CreateSearchState() => new EnemySearchState(_behavior);
    public EnemyIdleState CreateIdleState() => new EnemyIdleState(_behavior);
}