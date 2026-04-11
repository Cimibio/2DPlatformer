using UnityEngine;

public interface IChasable
{
    Transform transform { get; }
    bool IsAlive { get; }
}
