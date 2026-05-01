using UnityEngine;
using System.Collections;

namespace UI.Views
{
    public class SmoothCooldownBarView : CooldownBarView
    {
        [SerializeField] private float _smoothSpeed = 25f;

        private Coroutine _smoothCoroutine;
        private float _targetValue = 1f;

        protected override void UpdateDisplay(float normalizedValue)
        {
            _targetValue = normalizedValue;

            if (_smoothCoroutine != null)
                StopCoroutine(_smoothCoroutine);

            _smoothCoroutine = StartCoroutine(SmoothUpdate());
        }

        private IEnumerator SmoothUpdate()
        {
            while (Mathf.Abs(_slider.value - _targetValue) > 0.001f)
            {
                _slider.value = Mathf.MoveTowards(_slider.value, _targetValue, _smoothSpeed * Time.deltaTime);
                yield return null;
            }

            _slider.value = _targetValue;
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