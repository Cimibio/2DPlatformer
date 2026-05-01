namespace UI.Views
{
    public abstract class InstantProgressBarView : ProgressBarView
    {
        protected override void UpdateDisplay(float normalizedValue)
        {
            _slider.value = normalizedValue;
        }
    }
}