using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemySearcher : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _searchRadius = 3f;
    [SerializeField] private bool _debugMode = true;

    private CircleCollider2D _searchArea;
    private List<Health> _enemiesInRange = new List<Health>();

    public bool HasAnyEnemy => _enemiesInRange.Count > 0;
    public float SearchRadius => _searchRadius;

    private void Awake()
    {
        _searchArea = GetComponent<CircleCollider2D>();
        _searchArea.isTrigger = true;
        _searchArea.radius = _searchRadius;
    }

    private void Update()
    {
        if (_searchArea.radius != _searchRadius)
            _searchArea.radius = _searchRadius;
    }

    public Health GetNearestEnemy()
    {
        _enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.IsAlive);

        if (_enemiesInRange.Count == 0)
            return null;

        Health nearest = null;
        float minSqrDistance = float.MaxValue;
        Vector2 currentPos = transform.position;

        foreach (var enemy in _enemiesInRange)
        {
            Vector2 direction = (Vector2)enemy.transform.position - currentPos;
            float sqrDistance = direction.sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private bool IsEnemy(Collider2D other)
    {
        return (_enemyLayer.value & (1 << other.gameObject.layer)) != 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsEnemy(other))
        {
            if (other.TryGetComponent(out Health enemyHealth) && !_enemiesInRange.Contains(enemyHealth))
            {
                _enemiesInRange.Add(enemyHealth);

                if (_debugMode)
                    Debug.Log($"[EnemySearcher] Enemy entered.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsEnemy(other))
        {
            if (other.TryGetComponent(out Health enemyHealth))
            {
                _enemiesInRange.Remove(enemyHealth);

                if (_debugMode)
                    Debug.Log($"[EnemySearcher] Enemy left.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        if (Application.isPlaying && _enemiesInRange != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var enemy in _enemiesInRange)
            {
                if (enemy != null)
                    Gizmos.DrawLine(transform.position, enemy.transform.position);
            }
        }
    }
}