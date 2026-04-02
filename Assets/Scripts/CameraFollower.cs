using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Bounds")]
    [SerializeField] private Vector2 _minBounds;
    [SerializeField] private Vector2 _maxBounds;
    [SerializeField] private bool _useBounds = true;

    private Camera _camera;
    private float _halfHeight;
    private float _halfWidth;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        if (_camera != null)
        {
            _halfHeight = _camera.orthographicSize;
            _halfWidth = _halfHeight * _camera.aspect;
        }

        //UpdateCameraSize();
        AutoDetectBounds();
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // Целевая позиция
        Vector2 targetPosition = (Vector2)_target.position + _offset;

        // Ограничения
        if (_useBounds)
        {
            float minX = _minBounds.x + _halfWidth;
            float maxX = _maxBounds.x - _halfWidth;
            float minY = _minBounds.y + _halfHeight;
            float maxY = _maxBounds.y - _halfHeight;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Плавное движение
        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, _smoothSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (_useBounds)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((_minBounds.x + _maxBounds.x) / 2, (_minBounds.y + _maxBounds.y) / 2, 0);
            Vector3 size = new Vector3(_maxBounds.x - _minBounds.x, _maxBounds.y - _minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }

    private void AutoDetectBounds()
    {
        // Ищем объект с тегом "Level" или "Ground"
        GameObject level = GameObject.FindGameObjectWithTag("Level");
        if (level == null) 
            level = GameObject.FindGameObjectWithTag("Ground");

        if (level != null)
        {
            Collider2D[] colliders = level.GetComponentsInChildren<Collider2D>();

            if (colliders.Length > 0)
            {
                Bounds bounds = colliders[0].bounds;

                for (int i = 1; i < colliders.Length; i++)
                {
                    bounds.Encapsulate(colliders[i].bounds);
                }

                _minBounds = bounds.min;
                _maxBounds = bounds.max;
                _useBounds = true;

                Debug.Log($"Camera bounds set to: Min={_minBounds}, Max={_maxBounds}");
            }
        }
    }
}