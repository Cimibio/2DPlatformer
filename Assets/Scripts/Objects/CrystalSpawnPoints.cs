using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawnPoints : MonoBehaviour
{
    [SerializeField] private List<CrystalSpawnPoint> _spawnPoints;

    public IReadOnlyList<CrystalSpawnPoint> Points => _spawnPoints;
}

