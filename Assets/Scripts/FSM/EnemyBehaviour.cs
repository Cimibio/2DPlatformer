using UnityEngine;

[RequireComponent(typeof(Enemy), typeof(PatrolMover), typeof(Chaser))]
[RequireComponent(typeof(TargetDetector), typeof(EnemyAttacker))]
public abstract class EnemyBehavior : MonoBehaviour
{
    protected Enemy _enemy;
    protected PatrolMover PatrolMover => _enemy.PatrolMover;
    protected Chaser Chaser => _enemy.Chaser;
    public EnemyAttacker Attacker { get; private set; }

    public Enemy Enemy => _enemy;

    protected virtual void Awake()
    {
        _enemy = GetComponent<Enemy>();
        Attacker = GetComponent<EnemyAttacker>();

        if (_enemy == null)        
            Debug.LogError($"[{gameObject.name}] EnemyBehavior requires Enemy component!");        
    }

    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected virtual void Update() { }
    public virtual void Init() { }
}