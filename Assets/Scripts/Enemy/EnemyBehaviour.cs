using UnityEngine;

[RequireComponent (typeof(Enemy), typeof(PatrolMover), typeof(Chaser))]
[RequireComponent(typeof(TargetDetector))]
public abstract class EnemyBehavior : MonoBehaviour
{
    protected Enemy _enemy;
    protected PatrolMover PatrolMover => _enemy.PatrolMover;
    protected Chaser Chaser => _enemy.Chaser;
    protected TargetDetector TargetDetector => _enemy.TargetDetector;

    protected virtual void Awake()
    {
        _enemy = GetComponent<Enemy>();

        if (_enemy == null)
        {
            Debug.LogError($"[{gameObject.name}] EnemyBehavior requires Enemy component!");
        }
    }

    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected virtual void Update() { }
    public virtual void Init() { }
}