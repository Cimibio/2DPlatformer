using UnityEngine;

public class Searcher : TargetChaser
{
    public bool IsSearching => _isMoving;

    [SerializeField] private float _searchSpeed = 3f;

    protected override void Awake()
    {
        base.Awake();
        _speed = _searchSpeed;
    }

    public void Search(Vector3 targetPosition)
    {
        MoveTo(targetPosition);
    }

    public void StopSearch()
    {
        StopMoving();
    }

    protected override Color GetGizmoColor()
    {
        return Color.blue;
    }
}