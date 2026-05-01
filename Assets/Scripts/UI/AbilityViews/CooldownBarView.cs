using UnityEngine;

namespace UI.Views
{
    public class CooldownBarView : ProgressBarView
    {
        [SerializeField] protected Spawners.PlayerSpawner _playerSpawner;

        protected PlayerAbilityVampirism _currentAbility;
        protected Player _currentPlayer;

        protected virtual void OnEnable()
        {
            if (_playerSpawner != null)
                _playerSpawner.PlayerSpawned += OnPlayerSpawned;
        }

        protected virtual void OnDisable()
        {
            if (_playerSpawner != null)
                _playerSpawner.PlayerSpawned -= OnPlayerSpawned;

            if (_currentPlayer != null)
                _currentPlayer.Died -= OnPlayerDied;
        }

        protected override void Start()
        {
            base.Start();

            if (_playerSpawner != null && _playerSpawner.CurrentPlayer != null)
                OnPlayerSpawned(_playerSpawner.CurrentPlayer);
        }

        protected virtual void Update()
        {
            UpdateCurrentProgress();
        }

        protected virtual void OnPlayerSpawned(Player newPlayer)
        {
            if (_currentPlayer != null)
                _currentPlayer.Died -= OnPlayerDied;

            _currentPlayer = newPlayer;
            _currentAbility = newPlayer.GetComponent<PlayerAbilityVampirism>();

            if (_currentPlayer != null)
                _currentPlayer.Died += OnPlayerDied;
        }

        protected virtual void OnPlayerDied()
        {
            _currentAbility = null;
            _currentPlayer = null;
            SetProgress(1f);
        }

        protected virtual void UpdateCurrentProgress()
        {
            if (_currentAbility == null)
            {
                if (_slider.value != 1f)
                    SetProgress(1f);
                return;
            }

            float progress = 1f;

            if (_currentAbility.IsActive)
                progress = _currentAbility.ActiveProgress;
            else if (_currentAbility.IsOnCooldown)
                progress = _currentAbility.CooldownProgress;

            SetProgress(progress);
        }

        protected override void UpdateDisplay(float normalizedValue)
        {
            // Переопределяется в дочерних классах (Instant/Smooth)
        }
    }
}