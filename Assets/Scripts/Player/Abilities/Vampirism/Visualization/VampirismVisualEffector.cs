using UnityEngine;

[RequireComponent(typeof(RadiusVisualizer), typeof(SpritePulser))]
public class VampirismVisualEffector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAbilityVampirism _ability;
    [SerializeField] private EnemySearcher _enemySearcher;

    [Header("Visibility Settings")]
    [SerializeField] private float _fadeSpeed = 5f;

    [Header("Colors")]
    [SerializeField] private Color _colorWithEnemies = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color _colorWithoutEnemies = Color.white;

    private SpritePulser _spritePulser;
    private RadiusVisualizer _radiusVisualizer;
    private float _currentAlpha = 0f;

    private void Awake()
    {
        if (_enemySearcher == null)        
            _enemySearcher = GetComponentInParent<EnemySearcher>();
        
        if (_ability == null)        
            _ability = GetComponentInParent<PlayerAbilityVampirism>();
        
        _radiusVisualizer = GetComponent<RadiusVisualizer>();
        _spritePulser = GetComponent<SpritePulser>();
    }

    private void Update()
    {
        _radiusVisualizer.SetVisibility(_ability.IsActive);
        ApplyColorEffect();
    }

    private void OnDisable()
    {
        _radiusVisualizer.SetVisibility(false);
    }

    private void ApplyColorEffect()
    {
        Color targetColor = _enemySearcher.HasAnyEnemy ? _colorWithEnemies : _colorWithoutEnemies;
        float targetAlpha = _ability.IsActive ? 1f : 0f;

        _currentAlpha = Mathf.MoveTowards(_currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

        float finalAlpha = _currentAlpha;

        if (_ability.IsActive)
            finalAlpha *= _spritePulser.GetCurrentAlphaMultiplier();

        Color finalColor = targetColor;
        finalColor.a = finalAlpha;

        _radiusVisualizer.SetColor(finalColor);
    }
}