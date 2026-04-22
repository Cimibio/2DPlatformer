using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyCombatState : EnemyState
{
    private EnemySubState _currentSubState;
    private readonly CombatStateFactory _combatStateFactory;
    private Dictionary<Type, EnemySubState> _subStatesCache;
    private Dictionary<SubStateType, Type> _subStateTypesMap;
    private bool _shouldReturnToPatrol;

    public EnemyCombatState(SlimeEnemyBehavior behavior) : base(behavior)
    {
        _combatStateFactory = new CombatStateFactory(behavior, this);
        InitializeSubStateTypeMap();
        InitializeSubStateCache();
    }

    public enum SubStateType
    {
        Chase,
        Attack,
        Search,
        Idle
    }

    public bool ShouldReturnToPatrol => _shouldReturnToPatrol;
    private bool HasVisibleTarget => _targeter.HasTarget && _targeter.IsTargetInVisionZone && _targeter.HasLineOfSight;
    private bool CanAttackTarget => _attacker != null && _attacker.CanAttack && _attacker.IsTargetInAttackRange();
    private bool HasTargetButNoLoS => _targeter.HasTarget && !HasVisibleTarget;

    public override void Update()
    {
        if (_shouldReturnToPatrol)
            return;

        UpdateSubState();
        _currentSubState?.Update();
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

    public override void Exit()
    {
        _currentSubState?.Exit();
        _currentSubState = null;

        if (_behavior.DebugMode)
            Debug.Log($"[{_enemy.name}] Exiting Combat mode");
    }

    public void Idle()
    {
        TransitionToSubState(GetSubStateByType(SubStateType.Idle));
    }

    private void InitializeSubStateTypeMap()
    {
        _subStateTypesMap = new Dictionary<SubStateType, Type>
        {
            { SubStateType.Chase, typeof(EnemyChaseState) },
            { SubStateType.Attack, typeof(EnemyAttackState) },
            { SubStateType.Search, typeof(EnemySearchState) },
            { SubStateType.Idle, typeof(EnemyIdleState) }
        };
    }

    private void InitializeSubStateCache()
    {
        _subStatesCache = new Dictionary<Type, EnemySubState>
        {
            { typeof(EnemyChaseState), _combatStateFactory.CreateChaseState() },
            { typeof(EnemyAttackState), _combatStateFactory.CreateAttackState() },
            { typeof(EnemySearchState), _combatStateFactory.CreateSearchState() },
            { typeof(EnemyIdleState), _combatStateFactory.CreateIdleState() }
        };
    }

    private SubStateType DetermineInitialSubStateType()
    {
        if (HasVisibleTarget)
            return GetCombatSubStateType();

        return SubStateType.Search;
    }

    private void UpdateSubState()
    {
        SubStateType newStateType = DetermineSubStateType();

        if (_shouldReturnToPatrol)
            return;

        Type targetStateType = _subStateTypesMap[newStateType];

        if (_currentSubState == null || _currentSubState.GetType() != targetStateType)
        {
            EnemySubState nextState = GetSubStateByType(newStateType);
            TransitionToSubState(nextState);
        }
    }

    private SubStateType DetermineSubStateType()
    {
        if (HasVisibleTarget)
            return GetCombatSubStateType();

        if (_currentSubState is EnemyIdleState idleState && !idleState.IsComplete)
            return SubStateType.Idle;

        if (_currentSubState is EnemySearchState searchState && !searchState.IsComplete)
            return SubStateType.Search;

        if (_currentSubState is EnemySearchState completedSearch && completedSearch.IsComplete)
            return SubStateType.Idle;

        if (!_targeter.IsTargetInVisionZone && _targeter.HasTarget)
        {
            if (_behavior.DebugMode)
                Debug.Log($"[{_enemy.name}] Target left vision zone, switching to Search");
            return SubStateType.Search;
        }

        _shouldReturnToPatrol = true;
        return GetCurrentSubStateType();
    }

    private SubStateType GetCombatSubStateType()
    {
        return CanAttackTarget ? SubStateType.Attack : SubStateType.Chase;
    }

    private SubStateType GetCurrentSubStateType()
    {
        if (_currentSubState == null)
            return SubStateType.Idle;

        foreach (var pair in _subStateTypesMap)
        {
            if (pair.Value == _currentSubState.GetType())
                return pair.Key;
        }

        return SubStateType.Idle;
    }

    private EnemySubState GetSubStateByType(SubStateType stateType)
    {
        Type type = _subStateTypesMap[stateType];
        return GetSubState(type);
    }

    private EnemySubState GetSubState(Type stateType)
    {
        if (_subStatesCache.TryGetValue(stateType, out EnemySubState state))
            return state;

        Debug.LogError($"[{_enemy.name}] Sub-state {stateType.Name} not found in cache!");
        return _subStatesCache[typeof(EnemyIdleState)];
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
}