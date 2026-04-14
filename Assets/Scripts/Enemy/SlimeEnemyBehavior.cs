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
        else if (!TargetDetector.HasTarget)        
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

                PatrolMover.StartPatrol();
                break;

            case State.Chase:
                if (_debugMode) 
                    Debug.Log($"[{_enemy.name}] Entering Chase state");

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
                // Чейзер уже движется к LastKnownPosition
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
                // Останавливаем чейзер, если он не был остановлен
                break;

            case State.SearchLastPosition:
                StopForgetTimer();
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
        float timer = 0;

        while (timer < _forgetDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (_debugMode)
            Debug.Log($"[{_enemy.name}] Forgot about target after {_forgetDelay} seconds.");

        ForgetTarget();
    }

    private void ForgetTarget()
    {
        StopForgetTimer();
        Chaser.StopChase();

        if (_enemy.IsAlive)        
            PatrolMover.StartPatrol();        
    }

    public override void Init()
    {
        base.Init();
        _currentState = State.Patrol;

        if (!TargetDetector.HasTarget)        
            PatrolMover.StartPatrol();        
    }
}