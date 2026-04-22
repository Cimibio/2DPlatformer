using UnityEngine;

public class Chaser : TargetChaser
{
    public bool IsChasing => _isMoving;

    [SerializeField] private float _chaseSpeed = 4f;

    protected override void Awake()
    {
        base.Awake();
        _speed = _chaseSpeed;
    }

    public void Chase(Vector3 targetPosition)
    {
        MoveTo(targetPosition);
    }

    public void StopChase()
    {
        StopMoving();
    }

    protected override Color GetGizmoColor()
    {
        return Color.red;
    }
}