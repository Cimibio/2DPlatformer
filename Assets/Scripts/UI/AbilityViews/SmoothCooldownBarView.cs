using System.Collections;
using UnityEngine;

namespace AbilityView
{
    public class SmoothCooldownBarView : CooldownBarView
    {
        [SerializeField] private float _smoothSpeed = 50f;

        private float _currentDisplayValue = 1f;
        private Coroutine _smoothCoroutine;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_smoothCoroutine != null)
            {
                StopCoroutine(_smoothCoroutine);
                _smoothCoroutine = null;
            }
        }

        protected override void UpdateDisplay()
        {
            float targetValue = GetCurrentProgress();

            if (Mathf.Abs(_currentDisplayValue - targetValue) > 0)
            {
                _currentDisplayValue = targetValue;

                if (_smoothCoroutine != null)
                    StopCoroutine(_smoothCoroutine);

                _smoothCoroutine = StartCoroutine(SmoothUpdate());
            }
        }

        private IEnumerator SmoothUpdate()
        {
            while (Mathf.Approximately(_slider.value, _currentDisplayValue) == false)
            {
                _slider.value = Mathf.MoveTowards(_slider.value, _currentDisplayValue, _smoothSpeed * Time.deltaTime);
                yield return null;
            }

            _slider.value = _currentDisplayValue;
            _smoothCoroutine = null;
        }
    }
}