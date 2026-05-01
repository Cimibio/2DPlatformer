using System.Collections;
using UnityEngine;

namespace AbilityView
{
    public class SmoothCooldownBarView : CooldownBarView
    {
        [SerializeField] private float _smoothSpeed = 25f;

        private float _currentDisplayValue = 1f;
        private Coroutine _smoothCoroutine;

        private void Update()
        {
            UpdateDisplay();
        }

        protected override void UpdateDisplay()
        {
            if (_ability == null) 
                return;

            float targetValue;

            if (_ability.IsActive)
            {
                targetValue = _ability.ActiveProgress;
            }
            else if (_ability.IsOnCooldown)
            {
                targetValue = _ability.CooldownProgress;
            }
            else
            {
                targetValue = 1f;
            }

            if (Mathf.Abs(_currentDisplayValue - targetValue) > 0.001f)
            {
                _currentDisplayValue = targetValue;

                if (_smoothCoroutine != null)
                    StopCoroutine(_smoothCoroutine);

                _smoothCoroutine = StartCoroutine(SmoothUpdate());
            }
        }

        private IEnumerator SmoothUpdate()
        {
            float startValue = _slider.value;
            float elapsed = 0f;
            float duration = 1f / _smoothSpeed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float time = elapsed / duration;
                _slider.value = Mathf.Lerp(startValue, _currentDisplayValue, time);
                yield return null;
            }

            _slider.value = _currentDisplayValue;
            _smoothCoroutine = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_smoothCoroutine != null)
            {
                StopCoroutine(_smoothCoroutine);
                _smoothCoroutine = null;
            }
        }
    }
}