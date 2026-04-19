public class CombatStateFactory
{
    private readonly SlimeEnemyBehavior _behavior;
    private readonly EnemyCombatState _combatState;

    public CombatStateFactory(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
    {
        _behavior = behavior;
        _combatState = combatState;
    }

    public EnemyChaseState CreateChaseState() => new EnemyChaseState(_behavior, _combatState);
    public EnemyAttackState CreateAttackState() => new EnemyAttackState(_behavior, _combatState);
    public EnemySearchState CreateSearchState() => new EnemySearchState(_behavior, _combatState);
    public EnemyIdleState CreateIdleState() => new EnemyIdleState(_behavior, _combatState);
}