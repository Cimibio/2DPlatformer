using UnityEngine;

public class RadiusScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySearcher _enemySearcher;
    [SerializeField] private RadiusVisualizer _radiusVisualizer;

    [Header("Scale Settings")]
    [SerializeField] private bool _scaleOnStart = true;
    [SerializeField] private bool _scaleOnUpdate = true;
    [SerializeField] private float _scaleMultiplier = 2f;

    private float _currentRadius;
    private float _lastAppliedRadius = -1f;

    private void Awake()
    {
        if (_enemySearcher == null)
            _enemySearcher = GetComponentInParent<EnemySearcher>();

        if (_radiusVisualizer == null)
            _radiusVisualizer = GetComponent<RadiusVisualizer>();
    }

    private void Start()
    {
        if (_scaleOnStart && _enemySearcher != null)
        {
            _currentRadius = _enemySearcher.SearchRadius;
            ApplyScale(_currentRadius);
        }
    }

    private void Update()
    {
        if (!_scaleOnUpdate || _enemySearcher == null)
            return;

        float currentRadius = _enemySearcher.SearchRadius;

        // Если радиус изменился
        if (Mathf.Abs(currentRadius - _currentRadius) > 0.001f)
        {
            _currentRadius = currentRadius;
            ApplyScale(_currentRadius);
        }
    }

    public void UpdateScale(float radius)
    {
        _currentRadius = radius;
        ApplyScale(radius);
    }

    private void ApplyScale(float radius)
    {
        // Проверяем, не применяли ли уже такой же масштаб
        if (Mathf.Abs(_lastAppliedRadius - radius) < 0.001f)
            return;

        _lastAppliedRadius = radius;
        float scale = radius * _scaleMultiplier;

        if (_radiusVisualizer != null)
        {
            _radiusVisualizer.UpdateScale(radius);
        }
        else
        {
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        Debug.Log($"[RadiusScaler] Scale applied: radius={radius}, scale={scale}");
    }

    public void SetScaleMultiplier(float multiplier)
    {
        _scaleMultiplier = multiplier;
        ApplyScale(_currentRadius);
    }

    private void OnValidate()
    {
        if (_enemySearcher != null && Application.isPlaying == false)
        {
            ApplyScale(_enemySearcher.SearchRadius);
        }
    }
}