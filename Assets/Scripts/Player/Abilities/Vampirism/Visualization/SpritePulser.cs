using UnityEngine;

public class SpritePulser : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float _frequency = 2f;
    [SerializeField] private float _minAlpha = 0.5f;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private AnimationCurve _pulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float GetCurrentAlphaMultiplier()
    {
        float timeNormalized = (Mathf.Sin(Time.time * _frequency) + 1f) / 2f;
        float alphaMultiplier = Mathf.Lerp(_minAlpha, _maxAlpha, _pulseCurve.Evaluate(timeNormalized));
        return alphaMultiplier;
    }
}