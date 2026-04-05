using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoints : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnPoint> _spawnPoints;

    public IReadOnlyList<EnemySpawnPoint> Points => _spawnPoints;
}
