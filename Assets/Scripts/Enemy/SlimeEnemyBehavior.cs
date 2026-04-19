using System.Collections.Generic;
using UnityEngine;
using System;

public class SlimeEnemyBehavior : EnemyBehavior
{
    [Header("Slime Settings")]
    [SerializeField] private float _forgetDelay = 5f;
    [SerializeField] private float _stuckThreshold = 0.1f;
    [SerializeField] private float _stuckTime = 2f;
    [SerializeField] private bool _debugMode = true;

    private EnemyState _currentState;
    private StateFactory _stateFactory;
    private Dictionary<Type, EnemyState> _stateCache;

    public enum MainStateType
    {
        Patrol,
        Combat
    }

    public bool DebugMode => _debugMode;
    public float ForgetDelay => _forgetDelay;
    public float StuckThreshold => _stuckThreshold;
    public float StuckTime => _stuckTime;

    protected override void Awake()
    {
        base.Awake();
        _stateFactory = new StateFactory(this);
        InitializeStateCache();
    }

    protected override void Update()
    {
        if (!_enemy.IsAlive)
            return;

        UpdateMainState();
        _currentState?.Update();
    }

    public override void Init()
    {
        base.Init();
        TransitionToState(GetState<EnemyPatrolState>());

        if (!_enemy.Targeter.HasTarget)
            PatrolMover.Patrol();
    }

    private void InitializeStateCache()
    {
        _stateCache = new Dictionary<Type, EnemyState>
        {
            { typeof(EnemyPatrolState), _stateFactory.CreatePatrolState() },
            { typeof(EnemyCombatState), _stateFactory.CreateCombatState() }
        };
    }

    private void UpdateMainState()
    {
        if (!_enemy.IsAlive)
            return;

        MainStateType newStateType = DetermineMainStateType();
        Type targetStateType = newStateType == MainStateType.Patrol
            ? typeof(EnemyPatrolState)
            : typeof(EnemyCombatState);

        if (_currentState == null || _currentState.GetType() != targetStateType)
        {
            EnemyState nextState = GetState(targetStateType);
            TransitionToState(nextState);
        }
    }

    private MainStateType DetermineMainStateType()
    {
        if (!_enemy.IsAlive)
            return MainStateType.Patrol;

        if (_enemy.Targeter.HasTarget && _enemy.Targeter.HasLineOfSight)
            return MainStateType.Combat;

        if (_currentState is EnemyCombatState combatState && !combatState.ShouldReturnToPatrol)
            return MainStateType.Combat;

        return MainStateType.Patrol;
    }

    private EnemyState GetState<T>() where T : EnemyState
    {
        return GetState(typeof(T));
    }

    private EnemyState GetState(Type stateType)
    {
        if (_stateCache.TryGetValue(stateType, out EnemyState state))
        {
            return state;
        }

        Debug.LogError($"[{gameObject.name}] State {stateType.Name} not found in cache!");
        return _stateCache[typeof(EnemyPatrolState)];
    }

    private void TransitionToState(EnemyState newState)
    {
        if (_currentState == newState)
            return;

        if (_debugMode)
        {
            string oldStateName = _currentState?.GetType().Name ?? "None";
            Debug.Log($"[{_enemy.name}] Main State: {oldStateName} -> {newState.GetType().Name}");
        }

        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _currentState?.Exit();
    }
}