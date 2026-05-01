namespace UI.Views
{
    public class InstantCooldownBarView : CooldownBarView
    {
        protected override void UpdateDisplay(float normalizedValue)
        {
            _slider.value = normalizedValue;
        }
    }
}