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

        protected virtual void Awake()
        {
            _slider = GetComponent<Slider>();
            SetupSlider();
        }

        protected virtual void Start()
        {
            SetInitialValue();
            FindCurrentPlayerAbility();
        }

        protected virtual void Update()
        {
            if (_playerSpawner != null && _playerSpawner.CurrentPlayer != null)
            {
                var ability = _playerSpawner.CurrentPlayer.GetComponent<PlayerAbilityVampirism>();

                if (ability != _currentAbility)                
                    _currentAbility = ability;                
            }

            UpdateDisplay();
        }

        protected virtual void FindCurrentPlayerAbility()
        {
            if (_playerSpawner != null && _playerSpawner.CurrentPlayer != null)
            {
                _currentAbility = _playerSpawner.CurrentPlayer.GetComponent<PlayerAbilityVampirism>();
            }
        }

        protected virtual void SetupSlider()
        {
            _slider.minValue = 0;
            _slider.maxValue = 1;
            _slider.wholeNumbers = false;
        }

        protected virtual void SetInitialValue()
        {
            _slider.value = 1;
        }

        protected abstract void UpdateDisplay();

        protected float GetCurrentProgress()
        {
            if (_currentAbility == null) 
                return 1f;

            if (_currentAbility.IsActive)
                return _currentAbility.ActiveProgress;
            else if (_currentAbility.IsOnCooldown)
                return _currentAbility.CooldownProgress;
            else
                return 1f;
        }
    }
}