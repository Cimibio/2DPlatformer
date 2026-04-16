using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    [SerializeField] private float _checkRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private CapsuleCollider2D _collider;

    private Rigidbody2D _rigidbody;
    private bool _isFacingRight = true;

    public event Action Flipped;

    public bool IsFacingRight => _isFacingRight;

    private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

    public void Move(float direction, float speed)
    {
        if (IsBeforeWall(direction))
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            return;
        }

        _rigidbody.velocity = new Vector2(direction * speed, _rigidbody.velocity.y);

        TurnAround(direction);
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
        RaycastHit2D raycastHit = Physics2D.Raycast(_collider.bounds.center, Vector2.down,
            _collider.bounds.extents.y + _checkRadius, _groundLayer);

        Color rayColor = raycastHit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(_collider.bounds.center, Vector2.down * (_collider.bounds.extents.y + _checkRadius), rayColor);

        return raycastHit.collider != null;
    }

    private void TurnAround(float direction)
    {
        if (direction == 0) 
            return;

        if ((direction > 0) != _isFacingRight)
        {
            _isFacingRight = !_isFacingRight;
            transform.rotation = Quaternion.Euler(0, _isFacingRight ? 0 : 180, 0);
            Flipped?.Invoke();
        }
    }

    public bool IsBeforeWall(float direction)
    {
        if (Mathf.Approximately(direction, 0))
            return false;

        Vector2 checkDirection = direction > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D raycastHit = Physics2D.Raycast(_collider.bounds.center, checkDirection,
            _collider.bounds.extents.x + _checkRadius, _groundLayer);

        Color rayColor = raycastHit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(_collider.bounds.center, checkDirection * (_collider.bounds.extents.x + _checkRadius), rayColor);

        return raycastHit.collider != null;
    }
}

