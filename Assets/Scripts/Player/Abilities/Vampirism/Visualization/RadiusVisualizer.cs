using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RadiusVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySearcher _enemySearcher;

    [Header("Visual Settings")]
    [SerializeField] private Color _idleColor = new Color(1f, 1f, 1f, 0.1f);
    [SerializeField] private Color _activeColor = new Color(1f, 0.2f, 0.2f, 0.3f);
    [SerializeField] private bool _autoUpdateScale = true;

    private SpriteRenderer _spriteRenderer;
    private float _currentRadius;

    public Color IdleColor => _idleColor;
    public Color ActiveColor => _activeColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_enemySearcher == null)
            _enemySearcher = GetComponentInParent<EnemySearcher>();

        if (_autoUpdateScale && _enemySearcher != null)
            UpdateScale(_enemySearcher.SearchRadius);
    }

    public void UpdateScale(float radius)
    {
        _currentRadius = radius;
        // Диаметр = радиус * 2
        float scale = radius * 2f;
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    public void SetColor(Color color)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = color;
    }

    public void SetActiveColor()
    {
        SetColor(_activeColor);
    }

    public void SetIdleColor()
    {
        SetColor(_idleColor);
    }

    public void SetVisibility(bool isVisible)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = isVisible;
    }

    public void SetVisibilityWithAlpha(bool isVisible, float alphaMultiplier = 1f)
    {
        if (_spriteRenderer == null) return;

        _spriteRenderer.enabled = true;
        Color color = _spriteRenderer.color;
        color.a = (isVisible ? _activeColor.a : _idleColor.a) * alphaMultiplier;
        _spriteRenderer.color = color;
    }

    private void OnValidate()
    {
        if (_autoUpdateScale && _enemySearcher != null && Application.isPlaying == false)
        {
            UpdateScale(_enemySearcher.SearchRadius);
        }
    }
}