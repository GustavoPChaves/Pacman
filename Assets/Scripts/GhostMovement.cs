using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CircleCollider2D))]
public class GhostMovement : MonoBehaviour
{
    [SerializeField]
    float _baseSpeed = 1;
    float _speed;
    Vector2 _direction;
    Animator _animator;
    Rigidbody2D _rigidbody2D;


    Vector2 _targetPosition;
    GhostAI _ghostAI;

    [SerializeField]
    private Vector2 _startPos;

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

    public float Speed { get { return _speed = GetSpeedBasedOnGhostState(); } private set => _speed = value; }

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _ghostAI = GetComponent<GhostAI>();
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
        Vector2 position = Vector2.MoveTowards(currentPos, _targetPosition, Speed);
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
        if(!isFrightened)
            _animator.SetBool("EndingFrightend", false);
        else
        {
            StartCoroutine(UnityUtils.DelayedAction(5, () => EndingFrightened()));
        }

        
    }

    public void EndingFrightened()
    {
        _animator.SetBool("EndingFrightend", true);

    }

    public void Dead(bool isDead)
    {
        _animator.SetBool("Dead", isDead);
    }

    public void VerticalTilt()
    {
        Vector2 currentPos = transform.position;
        Vector2 tiltPos = _startPos;
        tiltPos.y += Mathf.PingPong(Time.time, 1);
        Vector2 position = Vector2.MoveTowards(currentPos, tiltPos, Speed);
        _rigidbody2D.MovePosition(position);

    }

    public void ResetPosition()
    {
        _targetPosition = _startPos;
        transform.position = _startPos;
    }

    float GetSpeedBasedOnGhostState()
    {
        if (_ghostAI.CurrentState == GhostAI.GhostState.Dead)
            return _baseSpeed * 2f;
        if (_ghostAI.CurrentState == GhostAI.GhostState.Frightened)
            return _baseSpeed * 0.5f;
        return _baseSpeed;
    }

}


public static class UnityUtils
{

    public static IEnumerator DelayedAction(float time, Action action)
    {
        yield return new WaitForSecondsRealtime(time);
        action?.Invoke();
    }
}
