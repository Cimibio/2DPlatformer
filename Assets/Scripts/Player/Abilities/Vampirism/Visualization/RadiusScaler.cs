using UnityEngine;

public class RadiusScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySearcher _enemySearcher;
    [SerializeField] private RadiusVisualizer _radiusVisualizer;

    [Header("Scale Settings")]
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
        if (_enemySearcher != null)
        {
            _currentRadius = _enemySearcher.SearchRadius;
            SetScale(_currentRadius);
        }
    }

    private void Update()
    {
        if (!_scaleOnUpdate || _enemySearcher == null)
            return;

        float currentRadius = _enemySearcher.SearchRadius;

        if (Mathf.Abs(currentRadius - _currentRadius) > 0)
        {
            _currentRadius = currentRadius;
            SetScale(_currentRadius);
        }
    }

    private void SetScale(float radius)
    {
        if (Mathf.Abs(_lastAppliedRadius - radius) < 0)
            return;

        _lastAppliedRadius = radius;
        float scale = radius * _scaleMultiplier;

        if (_radiusVisualizer != null)        
            _radiusVisualizer.SetScale(radius);        
        else        
            transform.localScale = new Vector3(scale, scale, 1f);        
    }
}