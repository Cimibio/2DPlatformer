using System.Collections;
using UnityEngine;

namespace UI.Views
{
    public class SmoothProgressBarView : ProgressBarView
    {
        [SerializeField] private float _smoothSpeed = 15f;

        private Coroutine _smoothCoroutine;
        private float _currentDisplayValue = 1f;

        protected override void UpdateDisplay(float normalizedValue)
        {
            if (Mathf.Abs(_currentDisplayValue - normalizedValue) > 0.001f)
            {
                _currentDisplayValue = normalizedValue;

                if (_smoothCoroutine != null)
                    StopCoroutine(_smoothCoroutine);

                _smoothCoroutine = StartCoroutine(SmoothUpdate());
            }
        }

        private IEnumerator SmoothUpdate()
        {
            while (Mathf.Abs(_slider.value - _currentDisplayValue) > 0.001f)
            {
                _slider.value = Mathf.MoveTowards(_slider.value, _currentDisplayValue, _smoothSpeed * Time.deltaTime);
                yield return null;
            }

            _slider.value = _currentDisplayValue;
            _smoothCoroutine = null;
        }

        protected virtual void OnDisable()
        {
            if (_smoothCoroutine != null)
            {
                StopCoroutine(_smoothCoroutine);
                _smoothCoroutine = null;
            }
        }
    }
}