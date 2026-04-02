using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;

    private Vector3 _currentDirection;

    private void Update()
    {
        if (_currentDirection != Vector3.zero)
            transform.Translate(_currentDirection * _speed * Time.deltaTime, Space.World);
    }

    public void SetDirection(Vector3 direction)
    {
        _currentDirection = direction.normalized;
    }

    public Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}