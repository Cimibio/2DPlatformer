using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform GetTransform => transform;

    public void Init(Vector3 position)
    {
        transform.position = position;
    }
}
