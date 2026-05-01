using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Slider))]
    public abstract class ProgressBarView : MonoBehaviour
    {
        [SerializeField] protected Slider _slider;

        protected virtual void Awake()
        {
            if (_slider == null)
                _slider = GetComponent<Slider>();

            SetupSlider();
        }

        protected virtual void Start()
        {
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
            _slider.value = 1f;
        }

        protected abstract void UpdateDisplay(float normalizedValue);

        protected void SetProgress(float normalizedValue)
        {
            normalizedValue = Mathf.Clamp01(normalizedValue);
            UpdateDisplay(normalizedValue);
        }
    }
}