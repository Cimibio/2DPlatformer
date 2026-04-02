using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private CapsuleCollider2D _collider;

    private Rigidbody2D _rigidbody;
    private bool _isFacingRight = true;


    private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

    public void Move(float direction, float speed)
    {
        _rigidbody.velocity = new Vector2(direction * speed, _rigidbody.velocity.y);
        Flip(direction);
    }

    public void Jump(float force)
    {
        if (IsGrounded())
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, force);
        }
    }

    public bool IsGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.Raycast(_collider.bounds.center, Vector2.down,
            _collider.bounds.extents.y + extraHeight, _groundLayer);

        Color rayColor = raycastHit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(_collider.bounds.center, Vector2.down * (_collider.bounds.extents.y + extraHeight), rayColor);

        return raycastHit.collider != null;
    }

    private void Flip(float direction)
    {
        if ((direction > 0 && !_isFacingRight) || (direction < 0 && _isFacingRight))
        {
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}

