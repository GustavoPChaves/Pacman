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

    Vector2 _targetPosition;

    public Vector2 TargetPosition
    {
        get => _targetPosition;
        set
        {
            _targetPosition.x = Mathf.RoundToInt(value.x);
            _targetPosition.y = Mathf.RoundToInt(value.y);
        }
    }

    public Vector2 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            SetMovementAnimation(_direction);
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

    }
    void SetMovementAnimation(Vector2 direction)
    {
        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
    }

    public void MoveToTargetPosition()
    {
       
        Vector2 currentPos = transform.position;
        Vector2 position = Vector2.MoveTowards(currentPos, _targetPosition, _speed);
        _rigidbody2D.MovePosition(position);
    }
    public bool HasReachTarget()
    {
        Vector2 currentPos = transform.position;

        return Vector3.Distance(currentPos, _targetPosition) < 0.0000001;
    }

    public void Frightened(bool isFrightened)
    {
        _animator.SetBool("Frightened", isFrightened);
    }

}
