using System.Collections;
using UnityEngine;

namespace UI.Views
{
    public abstract class SmoothProgressBarView : ProgressBarView
    {
        [SerializeField] private float _smoothSpeed = 50f;

        private Coroutine _smoothCoroutine;
        private float _targetValue = 1f;

        protected virtual void OnDisable()
        {
            if (_smoothCoroutine != null)
            {
                StopCoroutine(_smoothCoroutine);
                _smoothCoroutine = null;
            }
        }

        protected override void UpdateDisplay(float normalizedValue)
        {
            _targetValue = normalizedValue;

            if (_smoothCoroutine != null)
                StopCoroutine(_smoothCoroutine);

            _smoothCoroutine = StartCoroutine(SmoothUpdate());
        }

        private IEnumerator SmoothUpdate()
        {
            while (Mathf.Abs(_slider.value - _targetValue) > 0)
            {
                _slider.value = Mathf.MoveTowards(_slider.value, _targetValue, _smoothSpeed * Time.deltaTime);
                yield return null;
            }

            _slider.value = _targetValue;
            _smoothCoroutine = null;
        }
    }
}