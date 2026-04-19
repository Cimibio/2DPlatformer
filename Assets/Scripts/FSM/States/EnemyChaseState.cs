using UnityEngine;

public class EnemyChaseState : EnemySubState
{
    public EnemyChaseState(SlimeEnemyBehavior behavior, EnemyCombatState combatState)
        : base(behavior, combatState) { }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] → Chase");

        _patrolMover.StopPatrol();
        _chaser.Chase(_targeter.Target.position);
    }

    public override void Update()
    {
        if (_chaser.IsChasing)
        {
            _chaser.UpdateTarget(_targeter.Target.position);
            _attacker.SetTarget(_targeter.Target);
        }
    }

    public override void Exit()
    {
        _chaser.StopChase();
    }
}