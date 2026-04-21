public abstract class EnemySubState
{
    protected readonly SlimeEnemyBehavior _behavior;
    protected readonly EnemyCombatState _combatState;
    protected readonly Enemy _enemy;
    protected readonly PatrolMover _patrolMover;
    protected readonly Chaser _chaser;
    protected readonly EnemyAttacker _attacker;
    protected readonly TargetDetector _targeter;
    protected readonly Searcher _searcher;

    public EnemySubState(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
    {
        _behavior = behavior;
        _combatState = combatState;
        _enemy = behavior.Enemy;
        _patrolMover = _enemy.PatrolMover;
        _chaser = _enemy.Chaser;
        _attacker = behavior.Attacker;
        _targeter = _enemy.Targeter;
        _searcher = _enemy.Searcher;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}