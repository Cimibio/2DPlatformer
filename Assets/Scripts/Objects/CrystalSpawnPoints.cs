using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawnPoints : MonoBehaviour, ISpawnPointsContainer<CrystalSpawnPoint>
{
    [SerializeField] private List<CrystalSpawnPoint> _spawnPoints;

    public IReadOnlyList<CrystalSpawnPoint> Points => _spawnPoints;
}

