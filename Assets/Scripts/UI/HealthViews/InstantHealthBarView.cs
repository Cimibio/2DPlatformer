namespace UI.Views
{
    public class InstantHealthBarView : HealthBarView
    {
        protected override void UpdateDisplay(float normalizedValue)
        {
            _slider.value = normalizedValue;
        }
    }
}