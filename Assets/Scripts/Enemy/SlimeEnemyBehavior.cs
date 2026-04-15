using System.Collections;
using UnityEngine;

public class SlimeEnemyBehavior : EnemyBehavior
{
    [Header("Slime Settings")]
    [SerializeField] private float _forgetDelay = 5f;
    [SerializeField] private bool _debugMode = true;

    private State _currentState;
    private State _defaultState = State.Patrol;
    private Coroutine _forgetCoroutine;
    private bool _isForgetting = false;

    private enum State
    {
        Patrol,
        Chase,
        Attack,
        SearchLastPosition
    }

    protected override void Update()
    {
        if (!_enemy.IsAlive) 
            return;

        UpdateState();
        ExecuteCurrentState();
    }

    public override void Init()
    {
        base.Init();
        _currentState = _defaultState;
        _isForgetting = false;

        if (!_enemy.Targeter.HasTarget)
            PatrolMover.StartPatrol();
    }

    private void UpdateState()
    {
        if (!_enemy.IsAlive)
            return;

        State newState = _currentState;

        // Приоритеты состояний
        if (_enemy.Targeter.HasTarget && _enemy.Targeter.HasLineOfSight)
        {
            // Проверяем, можем ли атаковать
            if (Attacker != null && Attacker.CanAttack && Attacker.IsTargetInAttackRange())
            {
                newState = State.Attack;
                
            }
            else
            {
                newState = State.Chase;
            }
        }
        else if (_enemy.Targeter.HasTarget && !_enemy.Targeter.HasLineOfSight)
        {
            newState = State.SearchLastPosition;
        }
        else if (!_enemy.Targeter.HasTarget && !_isForgetting)
        {
            newState = State.Patrol;
        }

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
                StopForgetCountdown();
                Chaser.StopChase();
                Attacker?.ClearTarget();
                PatrolMover.StartPatrol();
                break;

            case State.Chase:
                if (_debugMode)
                    Debug.Log($"[{_enemy.name}] Entering Chase state");

                _isForgetting = false;
                StopForgetCountdown();
                PatrolMover.StopPatrol();
                Chaser.StartChase(_enemy.Targeter.Target.position);
                Attacker?.SetTarget(_enemy.Targeter.Target);
                break;

            case State.Attack:
                if (_debugMode)
                    Debug.Log($"[{_enemy.name}] Entering Attack state");

                PatrolMover.StopPatrol();
                Chaser.StopChase();
                Attacker?.SetTarget(_enemy.Targeter.Target);

                PerformAttack();
                break;

            case State.SearchLastPosition:
                if (_debugMode)
                    Debug.Log($"[{_enemy.name}] Entering Search state");

                Chaser.StartChase(_enemy.Targeter.LastKnownPosition);
                Attacker?.ClearTarget();
                BeginForgetCountdown();
                break;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (_currentState)
        {
            case State.Chase:
                UpdateChaseState();
                break;

            case State.Attack:
                UpdateAttackState();
                break;

            case State.SearchLastPosition:
                // Чейзер сам движется к последней позиции
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
            case State.Attack:
                // Выходим из атаки, сбрасываем флаги если нужно
                break;

            case State.SearchLastPosition:
                // Не останавливаем таймер при выходе
                break;
        }
    }

    private void UpdateChaseState()
    {
        if (Chaser.IsChasing)
        {
            Chaser.UpdateTarget(_enemy.Targeter.Target.position);
            Attacker?.SetTarget(_enemy.Targeter.Target);
        }
    }

    private void UpdateAttackState()
    {
        // Проверяем, всё ещё ли цель в зоне атаки и можем ли атаковать
        if (Attacker == null || !_enemy.Targeter.HasTarget)
            return;
        
        // Если цель вышла из зоны атаки или атака завершилась
        if (!Attacker.IsTargetInAttackRange() || !Attacker.CanAttack)
            return;
        
        // Обновляем позицию цели (вдруг сместилась)
        Attacker.SetTarget(_enemy.Targeter.Target);

        if (Attacker.CanAttack)        
            PerformAttack();        
    }

    private void PerformAttack()
    {
        if (Attacker != null && Attacker.CanAttack)
        {
            Attacker.Attack();

            // Можно добавить анимацию атаки
            // _animator.PlayAttackAnimation();
        }
    }

    private void BeginForgetCountdown()
    {
        StopForgetCountdown();
        _forgetCoroutine = StartCoroutine(ForgetCountdownCoroutine());
    }

    private void StopForgetCountdown()
    {
        if (_forgetCoroutine != null)
        {
            StopCoroutine(_forgetCoroutine);
            _forgetCoroutine = null;
        }
    }

    private IEnumerator ForgetCountdownCoroutine()
    {
        _isForgetting = true;
        float timer = 0;

        while (timer < _forgetDelay)
        {
            if (_enemy.Targeter.HasLineOfSight)
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

        _currentState = _defaultState;
        EnterState(_currentState);
    }    
}