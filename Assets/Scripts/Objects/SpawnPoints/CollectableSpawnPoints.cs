using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawnPoints : MonoBehaviour, ISpawnPointsContainer<CollectableSpawnPoint>
{
    [SerializeField] private List<CollectableSpawnPoint> _spawnPoints;

    public IReadOnlyList<CollectableSpawnPoint> Points => _spawnPoints;
}

