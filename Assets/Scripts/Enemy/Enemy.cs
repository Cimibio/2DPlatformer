using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMover), typeof(Chaser), typeof(TargetDetector))]
[RequireComponent(typeof(FallDetector), typeof(EnemyAnimator), typeof(SlimeEnemyBehavior))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Combat Settings")]
    [SerializeField] private float _maxHealth = 30f;

    private float _currentHealth;
    private PatrolMover _patrolMover;
    private Chaser _chaser;
    private TargetDetector _targeter;
    private FallDetector _fallDetector;
    private EnemyBehavior _behavior;
    private EnemyAnimator _animator;

    public event Action<Enemy> Falled;
    public event Action<float, float> HealthChanged;
    public event Action<Enemy> Died;

    public PatrolMover PatrolMover => _patrolMover;
    public Chaser Chaser => _chaser;
    public TargetDetector Targeter => _targeter;
    public bool IsAlive => _currentHealth > 0;

    private void Awake()
    {
        _patrolMover = GetComponent<PatrolMover>();
        _chaser = GetComponent<Chaser>();
        _targeter = GetComponent<TargetDetector>();
        _fallDetector = GetComponent<FallDetector>();
        _animator = GetComponent<EnemyAnimator>();
        _behavior = GetComponent<EnemyBehavior>();

        _currentHealth = _maxHealth;
    }

    private void OnEnable()
    {
        _fallDetector.Falled += Fall;
        _animator.EnemyDeathAnimationCompleted += Die;
    }

    private void OnDisable()
    {
        _fallDetector.Falled -= Fall;
        _animator.EnemyDeathAnimationCompleted -= Die;
    }

    public void Init(IReadOnlyList<Transform> patrolPoints)
    {
        _patrolMover.SetPatrolPoints(patrolPoints);
        _behavior.Init();
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) 
            return;

        _currentHealth -= damage;
        HealthChanged?.Invoke(_currentHealth, _maxHealth);

        Debug.Log($"[{gameObject.name}] Took {damage} damage. Health: {_currentHealth}/{_maxHealth}");

        if (_animator != null)        
            _animator.PlayHitAnimation();        

        if (_currentHealth <= 0)
            _animator.PlayDieAnimation();        
    }

    private void Die()
    {
        _currentHealth = 0;

        Debug.Log($"[{gameObject.name}] Died!");

        _chaser.StopChase();
        _patrolMover.StopPatrol();

        Died?.Invoke(this);
    }

    private void Fall()
    {
        Falled?.Invoke(this);
    }
}