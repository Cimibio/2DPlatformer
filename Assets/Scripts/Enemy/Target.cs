using UnityEngine;

[RequireComponent(typeof(EnemyMover), typeof(Route), typeof(Renderer))]
public class Target : MonoBehaviour
{
    private EnemyMover _mover;
    private Route _route;
    private Renderer _renderer;
    private Vector3 _direction;
    private int _startNextRoutePointNumber = 0;
    private int _currentNextRoutePointNumber;

    private void Awake()
    {
        _mover = GetComponent<EnemyMover>();
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _direction = GetDirection();
        _mover.SetDirection(_direction);
    }

    public void Init(Vector3 position, Route route, Color color)
    {
        transform.position = position;
        _renderer.material.color = color;
        _route = route;

        _currentNextRoutePointNumber = _startNextRoutePointNumber + 1;

        if (_route.RoutePoints.Count > 1)
        {
            _direction = (_route.RoutePoints[_currentNextRoutePointNumber] - position).normalized;
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 targetPoint = _route.RoutePoints[_currentNextRoutePointNumber];

        if ((transform.position - targetPoint).sqrMagnitude < 0.01f)
        {
            _currentNextRoutePointNumber = (_currentNextRoutePointNumber + 1) % _route.RoutePoints.Count;

            targetPoint = _route.RoutePoints[_currentNextRoutePointNumber];
        }

        return (targetPoint - transform.position).normalized;
    }
}
