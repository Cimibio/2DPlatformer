using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Target Target {  get; private set; }
    public Color Color {  get; private set; }
    public Transform GetTransform => transform;

    public void Init(Vector3 position, Color color, Target target)
    {
        transform.position = position;
        Color = color;
        Target = target;
    }
}
