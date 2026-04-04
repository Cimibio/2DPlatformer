using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> _spawnPoints;

    public IReadOnlyList<SpawnPoint> Points => _spawnPoints;
}
