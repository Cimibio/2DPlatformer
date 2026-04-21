public abstract class EnemyState
{
    protected readonly SlimeEnemyBehavior _behavior;
    protected readonly Enemy _enemy;
    protected readonly PatrolMover _patrolMover;
    protected readonly Chaser _chaser;
    protected readonly Searcher _searcher;
    protected readonly EnemyAttacker _attacker;
    protected readonly TargetDetector _targeter;

    public EnemyState(SlimeEnemyBehavior behavior)
    {
        _behavior = behavior;
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