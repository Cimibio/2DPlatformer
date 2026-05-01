namespace UI.Views
{
    public class InstantProgressBarView : ProgressBarView
    {
        protected override void UpdateDisplay(float normalizedValue)
        {
            _slider.value = normalizedValue;
        }
    }
}