using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [RequireComponent(typeof(Slider))]
    public abstract class ProgressBarView : MonoBehaviour
    {
        [SerializeField] protected Slider _slider;
        [SerializeField] protected float _inititalValue = 1f;

        protected virtual void Awake()
        {
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
            _slider.maxValue = _inititalValue;
            _slider.wholeNumbers = false;
        }

        protected virtual void SetInitialValue()
        {
            _slider.value = _inititalValue;
        }

        protected abstract void UpdateDisplay(float normalizedValue);

        protected void SetProgress(float normalizedValue)
        {
            normalizedValue = Mathf.Clamp01(normalizedValue);
            UpdateDisplay(normalizedValue);
        }
    }
}