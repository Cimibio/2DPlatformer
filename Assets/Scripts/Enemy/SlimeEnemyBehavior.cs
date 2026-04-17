using System.Collections.Generic;
using UnityEngine;
using System;

public class SlimeEnemyBehavior : EnemyBehavior
{
    [Header("Slime Settings")]
    [SerializeField] private float _forgetDelay = 5f;
    [SerializeField] private bool _debugMode = true;

    private EnemyState _currentState;
    private StateFactory _stateFactory;

    private Dictionary<Type, EnemyState> _stateCache;
    private Dictionary<StateType, Type> _stateTypeMap;

    public enum StateType
    {
        Patrol,
        Chase,
        Attack,
        Search,
        Idle
    }

    public bool DebugMode => _debugMode;
    public float ForgetDelay => _forgetDelay;


    protected override void Awake()
    {
        base.Awake();

        _stateFactory = new StateFactory(this);
        InitializeStateTypeMap();
        InitializeStateCache();
    }

    protected override void Update()
    {
        if (!_enemy.IsAlive)
            return;

        UpdateState();
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
            { typeof(EnemyChaseState), _stateFactory.CreateChaseState() },
            { typeof(EnemyAttackState), _stateFactory.CreateAttackState() },
            { typeof(EnemySearchState), _stateFactory.CreateSearchState() },
            { typeof(EnemyIdleState), _stateFactory.CreateIdleState() }
        };
    }

    private void InitializeStateTypeMap()
    {
        _stateTypeMap = new Dictionary<StateType, Type>
        {
            { StateType.Patrol, typeof(EnemyPatrolState) },
            { StateType.Chase, typeof(EnemyChaseState) },
            { StateType.Attack, typeof(EnemyAttackState) },
            { StateType.Search, typeof(EnemySearchState) },
            { StateType.Idle, typeof(EnemyIdleState) }
        };
    }

    private void UpdateState()
    {
        if (!_enemy.IsAlive)
            return;

        StateType newStateType = DetermineStateType();
        Type targetStateType = _stateTypeMap[newStateType];

        if (_currentState == null || _currentState.GetType() != targetStateType)
        {
            EnemyState nextState = GetState(targetStateType);
            TransitionToState(nextState);
        }
    }

    private StateType DetermineStateType()
    {
        if (!_enemy.IsAlive)
            return GetCurrentStateType();

        if (_currentState is EnemyIdleState idleState && !idleState.IsIdleComplete)
        {
            if (_enemy.Targeter.HasTarget && _enemy.Targeter.HasLineOfSight)
            {
                if (Attacker != null && Attacker.CanAttack && Attacker.IsTargetInAttackRange())
                    return StateType.Attack;
                else
                    return StateType.Chase;
            }

            return StateType.Idle;
        }

        if (_currentState is EnemySearchState searchState && !searchState.HasReachedPosition)
        {
            return StateType.Search;
        }

        if (_enemy.Targeter.HasTarget && _enemy.Targeter.HasLineOfSight)
        {
            if (Attacker != null && Attacker.CanAttack && Attacker.IsTargetInAttackRange())
                return StateType.Attack;
            else
                return StateType.Chase;

        }
        else if (_enemy.Targeter.HasTarget && !_enemy.Targeter.HasLineOfSight)
        {
            return StateType.Search;
        }

        return StateType.Patrol;
    }

    private StateType GetCurrentStateType()
    {
        if (_currentState == null)
            return StateType.Patrol;

        foreach (var pair in _stateTypeMap)
        {
            if (pair.Value == _currentState.GetType())
                return pair.Key;
        }

        return StateType.Patrol;
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
        {
            Debug.LogWarning($"[{gameObject.name}] Trying to transition to the same state: {newState.GetType().Name}");
            return;
        }

        if (_debugMode)
        {
            string oldStateName = _currentState?.GetType().Name ?? "None";
            Debug.Log($"[{_enemy.name}] State transition: {oldStateName} -> {newState.GetType().Name}");
        }

        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void OnSearchComplete()
    {
        if (_currentState is EnemySearchState)
        {
            TransitionToState(GetState<EnemyIdleState>());
        }
    }
    public void OnIdleComplete()
    {
        if (_currentState is EnemyIdleState && _enemy.IsAlive)
        {
            TransitionToState(GetState<EnemyPatrolState>());
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _currentState?.Exit();
    }
}