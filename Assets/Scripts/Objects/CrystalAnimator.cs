using System;
using UnityEngine;

public class CrystalAnimator : MonoBehaviour
{
    private Animator _animator;
    private string _animationDisapear = "isCollected";

    public event Action CollectionAnimationCompleted;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayDisapearAnimation()
    {
        _animator.SetTrigger(_animationDisapear);
    }

    public void OnCollectionAnimationComplete()
    {
        Debug.Log($"CrystalAnimator: Collection animation completed on {gameObject.name}");
        CollectionAnimationCompleted?.Invoke();
    }

    public void ResetToIdle()
    {
        _animator.Rebind();
        _animator.Update(0f);
    }
}
