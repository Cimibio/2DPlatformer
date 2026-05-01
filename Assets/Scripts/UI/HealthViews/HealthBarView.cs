using UnityEngine;

namespace UI.Views
{
    public class HealthBarView : ProgressBarView
    {
        [SerializeField] protected Health _health;
        [SerializeField] private bool _normalizeToMax = true;

        protected virtual void OnEnable()
        {
            if (_health != null)
            {
                _health.Changed += OnHealthChanged;
                SetupSlider();
                SetInitialValue();
            }
        }

        protected virtual void OnDisable()
        {
            if (_health != null)
                _health.Changed -= OnHealthChanged;
        }

        protected override void SetupSlider()
        {
            if (_normalizeToMax)
            {
                base.SetupSlider();
            }
            else
            {
                _slider.minValue = 0;
                _slider.maxValue = _health.Max;
                _slider.wholeNumbers = false;
            }
        }

        protected override void SetInitialValue()
        {
            float value = _normalizeToMax ? _health.Current / _health.Max : _health.Current;

            SetProgress(value);
        }

        private void OnHealthChanged(float current, float max)
        {
            float normalizedValue = _normalizeToMax ? current / max : current;
            SetProgress(normalizedValue);
        }

        protected override void UpdateDisplay(float normalizedValue)
        { }
    }
}