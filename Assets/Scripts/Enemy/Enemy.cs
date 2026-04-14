using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMover), typeof(Chaser), typeof(TargetDetector))]
[RequireComponent(typeof(FallDetector))]
public class Enemy : MonoBehaviour, IChasable
{
    private PatrolMover _patrolMover;
    private Chaser _chaser;
    private TargetDetector _targetDetector;
    private FallDetector _fallDetector;
    private EnemyBehavior _behavior;

    private bool _isAlive = true;

    public event Action<Enemy> Falled;

    // Публичные свойства для доступа из Behavior
    public PatrolMover PatrolMover => _patrolMover;
    public Chaser Chaser => _chaser;
    public TargetDetector TargetDetector => _targetDetector;
    public bool IsAlive => _isAlive;

    private void Awake()
    {
        _patrolMover = GetComponent<PatrolMover>();
        _chaser = GetComponent<Chaser>();
        _targetDetector = GetComponent<TargetDetector>();
        _fallDetector = GetComponent<FallDetector>();

        // Ищем компонент поведения на объекте
        _behavior = GetComponent<EnemyBehavior>();

        if (_behavior == null)
        {
            Debug.LogError($"[{gameObject.name}] No EnemyBehavior component found! Please add a behavior component.");
        }
    }

    private void OnEnable()
    {
        _fallDetector.Falled += OnFalled;
    }

    private void OnDisable()
    {
        _fallDetector.Falled -= OnFalled;
    }

    private void Update()
    {
        if (!_isAlive) 
            return;
    }

    public void Init(IReadOnlyList<Transform> patrolPoints)
    {
        _patrolMover.SetPatrolPoints(patrolPoints);
        _behavior?.Init();
    }

    private void OnFalled()
    {
        _isAlive = false;
        Falled?.Invoke(this);
    }
}