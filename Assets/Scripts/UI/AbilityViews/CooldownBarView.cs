using UnityEngine;
using UnityEngine.UI;

namespace AbilityView
{
    [RequireComponent(typeof(Slider))]
    public abstract class CooldownBarView : MonoBehaviour
    {
        [SerializeField] protected Spawners.PlayerSpawner _playerSpawner;

        protected Slider _slider;
        protected PlayerAbilityVampirism _currentAbility;
        protected Player _currentPlayer;

        private float _initialValue = 1f;

        protected virtual void Awake()
        {
            _slider = GetComponent<Slider>();
            SetupSlider();
        }

        protected virtual void Start()
        {
            SetInitialValue();

            if (_playerSpawner != null && _playerSpawner.CurrentPlayer != null)            
                SelectPlayer(_playerSpawner.CurrentPlayer);            
        }

        protected virtual void OnEnable()
        {
            if (_playerSpawner != null)            
                _playerSpawner.PlayerSpawned += SelectPlayer;            
        }

        protected virtual void OnDisable()
        {
            if (_playerSpawner != null)            
                _playerSpawner.PlayerSpawned -= SelectPlayer;            

            if (_currentPlayer != null)            
                _currentPlayer.Died -= ResetPlayer;            
        }

        protected virtual void SelectPlayer(Player newPlayer)
        {
            if (_currentPlayer != null)            
                _currentPlayer.Died -= ResetPlayer;            

            _currentPlayer = newPlayer;

            if (newPlayer.TryGetComponent(out PlayerAbilityVampirism vampirism))
                 _currentAbility = vampirism;

            if (_currentPlayer != null)            
                _currentPlayer.Died += ResetPlayer;            

            SetInitialValue();
        }

        protected virtual void ResetPlayer()
        {
            _currentAbility = null;
            _currentPlayer = null;
            SetInitialValue();
        }

        protected virtual void SetupSlider()
        {
            _slider.minValue = 0;
            _slider.maxValue = 1;
            _slider.wholeNumbers = false;
        }

        protected virtual void SetInitialValue()
        {
            _slider.value = _initialValue;
        }

        protected abstract void UpdateDisplay();

        protected float GetCurrentProgress()
        {
            if (_currentAbility == null) 
                return _initialValue;

            if (_currentAbility.IsActive)
                return _currentAbility.ActiveProgress;
            else if (_currentAbility.IsOnCooldown)
                return _currentAbility.CooldownProgress;
            else
                return _initialValue;
        }
    }
}