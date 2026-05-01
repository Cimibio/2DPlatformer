using UnityEngine;

public class RadiusVisualizerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAbilityVampirism _ability;
    [SerializeField] private RadiusVisualizer _radiusVisualizer;
    [SerializeField] private EnemySearcher _enemySearcher;

    [Header("Visibility Settings")]
    [SerializeField] private bool _showOnIdle = true;
    [SerializeField] private bool _showOnActive = true;
    [SerializeField] private bool _showOnlyWhenEnemiesInRange = true;
    [SerializeField] private float _fadeSpeed = 2f;

    private bool _shouldBeVisible;
    private float _currentAlpha;

    private void Awake()
    {
        if (_ability == null)
            _ability = GetComponentInParent<PlayerAbilityVampirism>();

        if (_radiusVisualizer == null)
            _radiusVisualizer = GetComponent<RadiusVisualizer>();

        if (_enemySearcher == null)
            _enemySearcher = GetComponentInParent<EnemySearcher>();
    }

    private void OnEnable()
    {
        if (_ability != null)
        {
            _ability.AbilityActivated += OnAbilityActivated;
            _ability.AbilityDeactivated += OnAbilityDeactivated;
        }
    }

    private void OnDisable()
    {
        if (_ability != null)
        {
            _ability.AbilityActivated -= OnAbilityActivated;
            _ability.AbilityDeactivated -= OnAbilityDeactivated;
        }

        if (_radiusVisualizer != null)
            _radiusVisualizer.SetVisibility(false);
    }

    private void Update()
    {
        UpdateVisibility();
        UpdateColorWithPulse();
    }

    private void UpdateVisibility()
    {
        if (_radiusVisualizer == null) 
            return;

        bool hasEnemies = _enemySearcher != null && _enemySearcher.HasAnyEnemy;
        bool isActive = _ability != null && _ability.IsActive;

        if (isActive)        
            _shouldBeVisible = _showOnActive;        
        else        
            _shouldBeVisible = _showOnIdle && (!_showOnlyWhenEnemiesInRange || hasEnemies);        

        float targetAlpha = _shouldBeVisible ? 1f : 0f;
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

        if (_currentAlpha > 0.01f)
        {
            _radiusVisualizer.SetVisibility(true);
            UpdateAlpha(_currentAlpha);
        }
        else
        {
            _radiusVisualizer.SetVisibility(false);
        }
    }

    private void UpdateColorWithPulse()
    {
        if (_radiusVisualizer == null) return;

        bool isActive = _ability != null && _ability.IsActive;
        bool hasEnemies = _enemySearcher != null && _enemySearcher.HasAnyEnemy;

        if (isActive)
        {
            float pulse = Mathf.Sin(Time.time * 5f) * 0.3f + 0.7f;
            Color color = new Color(1f, 0.2f, 0.2f, 0.3f * pulse);
            _radiusVisualizer.SetColor(color);
        }
        else if (hasEnemies && _showOnlyWhenEnemiesInRange)
        {
            float pulse = Mathf.Sin(Time.time * 2f) * 0.2f + 0.8f;
            Color color = new Color(1f, 1f, 1f, 0.15f * pulse);
            _radiusVisualizer.SetColor(color);
        }
        else
        {
            _radiusVisualizer.SetIdleColor();
        }
    }

    private void UpdateAlpha(float alpha)
    {
        if (_radiusVisualizer == null) return;

        bool isActive = _ability != null && _ability.IsActive;
        Color color = isActive ? _radiusVisualizer.ActiveColor : _radiusVisualizer.IdleColor;
        color.a = color.a * alpha;
        _radiusVisualizer.SetColor(color);
    }

    private void OnAbilityActivated()
    {
        if (_radiusVisualizer != null && _showOnActive)
        {
            _radiusVisualizer.SetActiveColor();
        }
    }

    private void OnAbilityDeactivated()
    {
        if (_radiusVisualizer != null && _showOnIdle)
        {
            _radiusVisualizer.SetIdleColor();
        }
    }
}