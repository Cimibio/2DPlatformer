using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMover), typeof(Chaser), typeof(TargetDetector))]
[RequireComponent(typeof(FallDetector), typeof(EnemyAnimator), typeof(SlimeEnemyBehavior))]
[RequireComponent(typeof(Health), typeof(Searcher))]
public class Enemy : MonoBehaviour
{
    private PatrolMover _patrolMover;
    private Chaser _chaser;
    private TargetDetector _targeter;
    private FallDetector _fallDetector;
    private EnemyBehavior _behavior;
    private EnemyAnimator _animator;
    private Health _health;
    private Searcher _searcher;

    public event Action<Enemy> Falled;
    public event Action<Enemy> Died;

    public PatrolMover PatrolMover => _patrolMover;
    public Chaser Chaser => _chaser;
    public TargetDetector Targeter => _targeter;
    public bool IsAlive => _health.IsAlive;
    public Health Health => _health;
    public Searcher Searcher => _searcher;

    private void Awake()
    {
        _patrolMover = GetComponent<PatrolMover>();
        _chaser = GetComponent<Chaser>();
        _targeter = GetComponent<TargetDetector>();
        _fallDetector = GetComponent<FallDetector>();
        _animator = GetComponent<EnemyAnimator>();
        _behavior = GetComponent<EnemyBehavior>();
        _health = GetComponent<Health>();
        _searcher = GetComponent<Searcher>();
    }

    private void OnEnable()
    {
        _fallDetector.Falled += Fall;
        _animator.EnemyDeathAnimationCompleted += NotifyDeathAnimationCompleted;
        _health.Died += Die;
    }

    private void OnDisable()
    {
        _fallDetector.Falled -= Fall;
        _animator.EnemyDeathAnimationCompleted -= NotifyDeathAnimationCompleted;
        _health.Died -= Die;
    }

    public void Init(IReadOnlyList<Transform> patrolPoints)
    {
        _patrolMover.SetPatrolPoints(patrolPoints);
        _behavior.Init();
        _health.Reset();
    }

    private void Die()
    {
        _chaser.StopChase();
        _patrolMover.StopPatrol();
        _animator.PlayDieAnimation();
    }

    private void NotifyDeathAnimationCompleted()
    {
        Debug.Log($"[{gameObject.name}] Died!");
        Died?.Invoke(this);
    }

    private void Fall()
    {        
        Falled?.Invoke(this);
    }
}