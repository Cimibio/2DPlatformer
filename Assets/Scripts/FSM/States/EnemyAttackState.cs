using UnityEngine;

public class EnemyAttackState : EnemySubState
{
    public EnemyAttackState(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
        : base(behavior, combatState) { }

    public override void Update()
    {
        if (!_targeter.HasTarget || !_targeter.Target.TryGetComponent<Health>(out var health) || !health.IsAlive)
            return;

        if (_attacker.CanAttack && _attacker.IsTargetInAttackRange())
            _attacker.Attack();
    }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] → Attack");

        _patrolMover.StopPatrol();
        _chaser.StopChase();
        _searcher.StopSearch();
        _attacker.SetTarget(_targeter.Target);
        _attacker.Attack();
    }
}