using UnityEngine;

namespace UI.Views
{
    public class InstantCooldownBarView : InstantProgressBarView
    {
        [SerializeField] protected Spawners.PlayerSpawner _playerSpawner;

        protected PlayerAbilityVampirism _currentAbility;
        protected Player _currentPlayer;

        protected virtual void OnEnable()
        {
            if (_playerSpawner != null)
                _playerSpawner.PlayerSpawned += UpdatePlayer;
        }

        protected override void Start()
        {
            base.Start();

            if (_playerSpawner != null && _playerSpawner.CurrentPlayer != null)
                UpdatePlayer(_playerSpawner.CurrentPlayer);
        }

        protected virtual void Update()
        {
            UpdateCurrentProgress();
        }

        protected virtual void OnDisable()
        {
            if (_playerSpawner != null)
                _playerSpawner.PlayerSpawned -= UpdatePlayer;

            if (_currentPlayer != null)
                _currentPlayer.Died -= ResetPlayer;
        }

        protected virtual void UpdatePlayer(Player newPlayer)
        {
            if (_currentPlayer != null)
                _currentPlayer.Died -= ResetPlayer;

            _currentPlayer = newPlayer;
            _currentAbility = newPlayer.GetComponent<PlayerAbilityVampirism>();

            if (_currentPlayer != null)
                _currentPlayer.Died += ResetPlayer;
        }

        protected virtual void ResetPlayer()
        {
            _currentAbility = null;
            _currentPlayer = null;
            SetInitialValue();
        }

        protected virtual void UpdateCurrentProgress()
        {
            if (_currentAbility == null)
            {
                if (_slider.value != _inititalValue)
                    SetInitialValue();
                return;
            }

            float progress = _inititalValue;

            if (_currentAbility.IsActive)
                progress = _currentAbility.ActiveProgress;
            else if (_currentAbility.IsOnCooldown)
                progress = _currentAbility.CooldownProgress;

            SetProgress(progress);
        }
    }
}