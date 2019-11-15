using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CircleCollider2D))]
public class GhostMovement : MonoBehaviour
{
    [SerializeField]
    float _speed = 1;
    Vector2 _direction;
    Animator _animator;
    Rigidbody2D _rigidbody2D;

    public Vector2 targetPosition;

    public Vector2 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            SetMovementAnimation(_direction);
            //_rigidbody2D.velocity = _speed * _direction;
            //targetPosition = _rigidbody2D.position + _direction;
            //Vector2 p = Vector2.MoveTowards(_rigidbody2D.position, _rigidbody2D.position + _direction, _speed);
            //_rigidbody2D.MovePosition(p);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Direction = _rigidbody2D.velocity;
    }
    void SetMovementAnimation(Vector2 direction)
    {
        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
    }


}
