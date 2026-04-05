using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Crystal : MonoBehaviour
{
    private CircleCollider2D _collider;

    public event Action<Crystal> Collected;

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Player>(out _))
        {
            Collected?.Invoke(this);
        }
    }
}
