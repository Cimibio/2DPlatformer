using System.Collections;
using UnityEngine;

public class SlimeEnemyBehavior : EnemyBehavior
{
    [Header("Slime Settings")]
    [SerializeField] private float _forgetDelay = 5f;
    [SerializeField] private bool _debugMode = true;

    private enum State
    {
        Patrol,
        Chase,
        SearchLastPosition
    }

    private State _currentState;
    private Coroutine _forgetCoroutine;
    private bool _isForgetting = false; // ← флаг процесса забывания

    protected override void Update()
    {
        UpdateState();
        ExecuteCurrentState();
    }

    private void UpdateState()
    {
        State newState = _currentState;

        if (TargetDetector.HasTarget && TargetDetector.HasLineOfSight)
            newState = State.Chase;
        else if (TargetDetector.HasTarget && !TargetDetector.HasLineOfSight)
            newState = State.SearchLastPosition;
        else if (!TargetDetector.HasTarget && !_isForgetting) // ← КЛЮЧЕВОЕ ИЗМЕНЕНИЕ
            newState = State.Patrol;

        if (newState != _currentState)
        {
            ExitState(_currentState);
            _currentState = newState;
            EnterState(_currentState);
        }
    }

    private void EnterState(State state)
    {
        switch (state)
        {
            case State.Patrol:
                if (_debugMode)
                    Debug.Log($"[{_enemy.name}] Entering Patrol state");

                _isForgetting = false;
                StopForgetTimer();
                Chaser.StopChase();
                PatrolMover.StartPatrol();
                break;

            case State.Chase:
                if (_debugMode)
                    Debug.Log($"[{_enemy.name}] Entering Chase state");

                _isForgetting = false;
                StopForgetTimer();
                PatrolMover.StopPatrol();
                Chaser.StartChase(TargetDetector.Target.position);
                break;

            case State.SearchLastPosition:
                if (_debugMode)
                    Debug.Log($"[{_enemy.name}] Entering Search state");

                Chaser.StartChase(TargetDetector.LastKnownPosition);
                StartForgetTimer();
                break;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (_currentState)
        {
            case State.Chase:
                if (Chaser.IsChasing)
                    Chaser.UpdateTarget(TargetDetector.Target.position);
                break;

            case State.SearchLastPosition:
                // Если цель вышла из зоны детекции, но мы ещё не забыли - продолжаем поиск
                // Таймер тикает сам
                break;

            case State.Patrol:
                // Патруль работает сам
                break;
        }
    }

    private void ExitState(State state)
    {
        switch (state)
        {
            case State.Chase:
                // Ничего не делаем
                break;

            case State.SearchLastPosition:
                // Не останавливаем таймер при выходе в Patrol, 
                // т.к. он должен дотикать сам
                break;
        }
    }

    private void StartForgetTimer()
    {
        StopForgetTimer();
        _forgetCoroutine = StartCoroutine(ForgetTimerCoroutine());
    }

    private void StopForgetTimer()
    {
        if (_forgetCoroutine != null)
        {
            StopCoroutine(_forgetCoroutine);
            _forgetCoroutine = null;
        }
    }

    private IEnumerator ForgetTimerCoroutine()
    {
        _isForgetting = true;
        float timer = 0;

        while (timer < _forgetDelay)
        {
            // Если цель снова стала видимой - прерываем забывание
            if (TargetDetector.HasLineOfSight)
            {
                if (_debugMode)
                    Debug.Log($"[{_enemy.name}] Target spotted again, canceling forget timer");

                _isForgetting = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Forgot about target after {_forgetDelay} seconds.");

        _isForgetting = false;

        // Цель забыта - переходим в Patrol
        _currentState = State.Patrol;
        EnterState(_currentState);
    }

    public override void Init()
    {
        base.Init();
        _currentState = State.Patrol;
        _isForgetting = false;

        if (!TargetDetector.HasTarget)
            PatrolMover.StartPatrol();
    }
}