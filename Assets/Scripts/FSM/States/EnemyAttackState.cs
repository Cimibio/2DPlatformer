using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(SlimeEnemyBehavior behavior) : base(behavior) { }

    public override void Enter()
    {
        Debug.Log($"[{_enemy.name}] Entering Attack state");

        _patrolMover.StopPatrol();
        _chaser.StopChase();
        _attacker.SetTarget(_targeter.Target);
        _attacker.Attack();
    }

    public override void Update()
    {
        if (_attacker.CanAttack && _attacker.IsTargetInAttackRange())
            _attacker.Attack();
    }
}
