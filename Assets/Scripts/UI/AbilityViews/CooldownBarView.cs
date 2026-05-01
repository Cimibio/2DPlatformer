using UnityEngine;
using UnityEngine.UI;

namespace AbilityView
{
    [RequireComponent(typeof(Slider))]
    public abstract class CooldownBarView : MonoBehaviour
    {
        [SerializeField] protected PlayerAbilityVampirism _ability;
        protected Slider _slider;

        protected virtual void Awake()
        {
            _slider = GetComponent<Slider>();
            SetupSlider();
        }

        protected virtual void Start()
        {
            SetInitialValue();
        }

        protected virtual void OnDisable()
        {

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
    }
}