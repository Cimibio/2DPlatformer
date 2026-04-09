using System.Collections.Generic;

public interface ISpawnPointsContainer<T> where T : ISpawnPoint
{
    public IReadOnlyList<T> Points { get; }
}
