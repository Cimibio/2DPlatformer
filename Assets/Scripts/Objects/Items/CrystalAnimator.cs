public class CrystalAnimator : CollectableAnimator
{
    public override void ResetToIdle()
    {
        _animator.Rebind();
        _animator.Update(0f);
    }
}
