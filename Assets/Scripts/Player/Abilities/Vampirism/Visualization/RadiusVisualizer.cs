using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RadiusVisualizer : MonoBehaviour
{
    [SerializeField] private EnemySearcher _enemySearcher;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_enemySearcher == null)
            _enemySearcher = GetComponentInParent<EnemySearcher>();

        if (_enemySearcher != null)
            SetScale(_enemySearcher.SearchRadius);
    }

    public void SetScale(float radius)
    {
        float scale = radius * 2f;
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void SetVisibility(bool isVisible)
    {
        _spriteRenderer.enabled = isVisible;
    }
}