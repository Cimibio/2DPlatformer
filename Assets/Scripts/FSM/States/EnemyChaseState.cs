using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(SlimeEnemyBehavior behavior) : base(behavior) { }

    public override void Enter()
    {
        Debug.Log($"[{_enemy.name}] Entering Chase state");

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
