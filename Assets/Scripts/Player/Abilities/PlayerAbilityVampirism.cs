using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader), typeof(Health), typeof(EnemySearcher))]
public class PlayerAbilityVampirism : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private float _duration = 6f;
    [SerializeField] private float _cooldown = 4f;
    [SerializeField] private float _damagePerSecond = 5f;
    [SerializeField] private float _healPerSecond = 4f;

    [Header("Tick Settings")]
    [SerializeField] private float _ticksPerSecond = 10f;

    private bool _isActive = false;
    private bool _isOnCooldown = false;
    private float _cooldownTimer = 0f;
    private float _damagePerTick;
    private float _healPerTick;
    private Coroutine _vampirismCoroutine;

    private PlayerInputReader _inputReader;
    private Health _playerHealth;
    private EnemySearcher _enemySearcher;
    private WaitForSeconds _tickDelay;

    public bool IsActive => _isActive;
    public bool IsOnCooldown => _isOnCooldown;
    public float CooldownProgress => _isOnCooldown ? 1f - (_cooldownTimer / _cooldown) : 1f;
    public float ActiveProgress => _isActive && _vampirismCoroutine != null ? 1f - (_activeTime / _duration) : 0f;

    private float _activeTime;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _playerHealth = GetComponent<Health>();
        _enemySearcher = GetComponent<EnemySearcher>();

        _damagePerTick = _damagePerSecond / _ticksPerSecond;
        _healPerTick = _healPerSecond / _ticksPerSecond;
        _tickDelay = new WaitForSeconds(1f / _ticksPerSecond);
    }

    private void Update()
    {
        UpdateCooldown();

        if (_isActive == false && _isOnCooldown == false && _inputReader.IsVampirismPressed)
            ActivateAbility();
    }

    private void OnDisable()
    {
        CancelAbility();
    }

    public void ActivateAbility()
    {
        _isActive = true;
        _activeTime = 0f;
        Debug.Log($"[Vampirism] Ability activated! Duration: {_duration} seconds");

        if (_vampirismCoroutine != null)
            StopCoroutine(_vampirismCoroutine);

        _vampirismCoroutine = StartCoroutine(VampirismCoroutine());
    }

    public void CancelAbility()
    {
        if (_isActive)
        {
            EndAbility();
            Debug.Log("[Vampirism] Ability cancelled!");
        }
    }

    private void UpdateCooldown()
    {
        if (_isOnCooldown)
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0f)
            {
                _isOnCooldown = false;
                Debug.Log("[Vampirism] Cooldown finished! Ability ready.");
            }
        }
    }

    private void EndAbility()
    {
        if (_vampirismCoroutine != null)
        {            
            StopCoroutine(_vampirismCoroutine);
            _vampirismCoroutine = null;
        }

        _isActive = false;
        _isOnCooldown = true;
        _cooldownTimer = _cooldown;

        Debug.Log($"[Vampirism] Ability ended. Cooldown: {_cooldown} seconds");
    }

    private IEnumerator VampirismCoroutine()
    {
        while (_activeTime < _duration)
        {
            if (_enemySearcher.HasAnyEnemy)
            {
                Health nearestEnemy = _enemySearcher.GetNearestEnemy();

                if (nearestEnemy != null && nearestEnemy.IsAlive)
                {
                    nearestEnemy.TakeDamage(_damagePerTick);
                    _playerHealth.Heal(_healPerTick);

                    Debug.Log($"[Vampirism] Drained {_damagePerTick:F1} HP from {nearestEnemy.name}, healed {_healPerTick:F1} HP. " +
                              $"Time: {_activeTime:F1}/{_duration}");
                }
            }
            else
            {
                Debug.Log($"[Vampirism] No enemies in range. Time: {_activeTime:F1}/{_duration}");
            }

            _activeTime += 1f / _ticksPerSecond;
            yield return _tickDelay;
        }

        EndAbility();
    }
}