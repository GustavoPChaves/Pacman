using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CircleCollider2D))]
public class PacmanController : MonoBehaviour
{

    Vector2 _direction;
    Animator _animator;
    Rigidbody2D _rigidbody2D;

    [SerializeField]
    float _speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _direction = Vector2.right;

        SetRigidbodyVelocity(_direction, _speed);
    }

    // Update is called once per frame
    void Update()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");

        SetRigidbodyVelocity(_direction, _speed);
        SetMovementAnimation(_direction);
    }

    void SetRigidbodyVelocity(Vector2 direction, float speed)
    {
        if(!direction.IsVectorZero())
            _rigidbody2D.velocity = speed * direction.normalized;
    }

    void SetMovementAnimation(Vector2 direction)
    {
        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
    }



}
