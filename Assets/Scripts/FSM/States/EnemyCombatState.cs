using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyCombatState : EnemyState
{
    private EnemySubState _currentSubState;
    private readonly CombatStateFactory _combatStateFactory;
    private Dictionary<Type, EnemySubState> _subStateCache;
    private Dictionary<SubStateType, Type> _subStateTypeMap;

    private bool _shouldReturnToPatrol;
    public bool ShouldReturnToPatrol => _shouldReturnToPatrol;

    public enum SubStateType
    {
        Chase,
        Attack,
        Search,
        Idle
    }

    public EnemyCombatState(SlimeEnemyBehavior behavior) : base(behavior)
    {
        _combatStateFactory = new CombatStateFactory(behavior, this);
        InitializeSubStateTypeMap();
        InitializeSubStateCache();
    }

    private void InitializeSubStateTypeMap()
    {
        _subStateTypeMap = new Dictionary<SubStateType, Type>
        {
            { SubStateType.Chase, typeof(EnemyChaseState) },
            { SubStateType.Attack, typeof(EnemyAttackState) },
            { SubStateType.Search, typeof(EnemySearchState) },
            { SubStateType.Idle, typeof(EnemyIdleState) }
        };
    }

    private void InitializeSubStateCache()
    {
        _subStateCache = new Dictionary<Type, EnemySubState>
        {
            { typeof(EnemyChaseState), _combatStateFactory.CreateChaseState() },
            { typeof(EnemyAttackState), _combatStateFactory.CreateAttackState() },
            { typeof(EnemySearchState), _combatStateFactory.CreateSearchState() },
            { typeof(EnemyIdleState), _combatStateFactory.CreateIdleState() }
        };
    }

    public override void Enter()
    {
        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Entering Combat mode");

        _shouldReturnToPatrol = false;

        SubStateType initialStateType = DetermineInitialSubStateType();
        EnemySubState initialState = GetSubStateByType(initialStateType);
        TransitionToSubState(initialState);
    }

    public override void Update()
    {
        if (_shouldReturnToPatrol)
            return;

        UpdateSubState();
        _currentSubState?.Update();
    }

    public override void Exit()
    {
        _currentSubState?.Exit();
        _currentSubState = null;

        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Exiting Combat mode");
    }

    private SubStateType DetermineInitialSubStateType()
    {
        if (_targeter.HasTarget && _targeter.HasLineOfSight)
        {
            if (_attacker != null && _attacker.CanAttack && _attacker.IsTargetInAttackRange())
                return SubStateType.Attack;
            else
                return SubStateType.Chase;
        }

        return SubStateType.Search;
    }

    private void UpdateSubState()
    {
        SubStateType newStateType = DetermineSubStateType();

        if (_shouldReturnToPatrol)
            return;

        Type targetStateType = _subStateTypeMap[newStateType];

        if (_currentSubState == null || _currentSubState.GetType() != targetStateType)
        {
            EnemySubState nextState = GetSubStateByType(newStateType);
            TransitionToSubState(nextState);
        }
    }

    private SubStateType DetermineSubStateType()
    {
        if (_currentSubState is EnemyIdleState idleState && !idleState.IsComplete)
        {
            if (_targeter.HasTarget && _targeter.HasLineOfSight)
            {
                return _attacker != null && _attacker.IsTargetInAttackRange()
                    ? SubStateType.Attack
                    : SubStateType.Chase;
            }

            return SubStateType.Idle;
        }

        if (_currentSubState is EnemySearchState searchState && !searchState.IsComplete)
        {
            if (_targeter.HasTarget && _targeter.HasLineOfSight)
            {
                return _attacker != null && _attacker.IsTargetInAttackRange()
                    ? SubStateType.Attack
                    : SubStateType.Chase;
            }

            return SubStateType.Search;
        }

        if (_targeter.HasTarget && _targeter.HasLineOfSight)
        {
            if (_attacker != null && _attacker.CanAttack && _attacker.IsTargetInAttackRange())
                return SubStateType.Attack;
            else
                return SubStateType.Chase;
        }
        else if (_targeter.HasTarget && !_targeter.HasLineOfSight)
        {
            return SubStateType.Search;
        }

        _shouldReturnToPatrol = true;
        return GetCurrentSubStateType();
    }

    private SubStateType GetCurrentSubStateType()
    {
        if (_currentSubState == null)
            return SubStateType.Idle;

        foreach (var pair in _subStateTypeMap)
        {
            if (pair.Value == _currentSubState.GetType())
                return pair.Key;
        }

        return SubStateType.Idle;
    }

    private EnemySubState GetSubStateByType(SubStateType stateType)
    {
        Type type = _subStateTypeMap[stateType];
        return GetSubState(type);
    }

    private EnemySubState GetSubState(Type stateType)
    {
        if (_subStateCache.TryGetValue(stateType, out EnemySubState state))
            return state;

        Debug.LogError($"[{_enemy.name}] Sub-state {stateType.Name} not found in cache!");
        return _subStateCache[typeof(EnemyIdleState)];
    }

    private void TransitionToSubState(EnemySubState newState)
    {
        if (_currentSubState == newState)
            return;

        if (_behavior.DebugMode)
        {
            string oldName = _currentSubState?.GetType().Name ?? "None";
            Debug.Log($"[{_enemy.name}] Combat Sub-state: {oldName} -> {newState.GetType().Name}");
        }

        _currentSubState?.Exit();
        _currentSubState = newState;
        _currentSubState?.Enter();
    }

    public void OnSearchStuck()
    {
        if (_currentSubState is EnemySearchState)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Stuck during search, going to Idle");

            TransitionToSubState(GetSubStateByType(SubStateType.Idle));
        }
    }

    public void OnReachedLastKnownPosition()
    {
        if (_currentSubState is EnemySearchState)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Reached last known position, going to Idle");

            TransitionToSubState(GetSubStateByType(SubStateType.Idle));
        }
    }

    public void OnIdleComplete()
    {
        if (_currentSubState is EnemyIdleState)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Idle complete, returning to Patrol");

            _shouldReturnToPatrol = true;
        }
    }
}