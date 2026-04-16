using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CollectableAnimator : MonoBehaviour
{
    protected Animator _animator;

    protected readonly int _disappearHash = Animator.StringToHash("isCollected");

    public event Action CollectionAnimationCompleted;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void PlayDisappearAnimation()
    {
        _animator.SetTrigger(_disappearHash);
    }

    public virtual void ResetToIdle()
    {
        _animator.Rebind();
        _animator.Update(0f);
    }

    public virtual void NotifyDisapearAnimationComplete()
    {
        CollectionAnimationCompleted?.Invoke();
    }
}