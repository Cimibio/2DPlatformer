using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private Transform _route;

    private List<Transform> _patrolPoints = new List<Transform>();

    public IReadOnlyList<Transform> PatrolPoints => _patrolPoints;

    private void Awake()
    {
        LoadPatrolPoints();
        Debug.Log($"Loaded {_patrolPoints.Count} points");
    }

    public void Init(Vector3 position)
    {
        transform.position = position;
    }

    public void RefreshPatrolPoints()
    {
        LoadPatrolPoints();
    }

    private void LoadPatrolPoints()
    {
        Debug.Log($"Loading Patrol points");

        _patrolPoints.Clear();

        if (_route == null)
        {
            Debug.Log($"Route is null");
            return;
        }

        foreach (Transform child in _route)
            _patrolPoints.Add(child);
    }
}
