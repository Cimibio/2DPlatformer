using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    private List<Vector3> _routePoints = new List<Vector3>();

    public IReadOnlyList<Vector3> RoutePoints => _routePoints;

    public void Add(Vector3 routePoint)
    {
        _routePoints.Add(routePoint);
    }

    public void Remove(int index) 
    {
        _routePoints.RemoveAt(index); 
    }
}
