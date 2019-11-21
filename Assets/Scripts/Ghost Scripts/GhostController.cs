using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible with physics, collision and animation of the Ghost.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CircleCollider2D))]
public class GhostController: MonoBehaviour
{
    //A small number needed to compare numbers without floating point inaccuracy doubt
    private const double _smallNumber = 0.0000001;

    [SerializeField]
    float _baseSpeed = 1;

    [SerializeField]
    private Vector2 _startPos;

    Vector2 _direction;
    Vector2 _targetPosition;

    Animator _animator;
    Rigidbody2D _rigidbody2D;
    GhostAI _ghostAI;

    /// <summary>
    /// TargetPosition of the ghost, need to be RoundToInt because is used in an array for pathfinding
    /// </summary>
    public Vector2 TargetPosition
    {
        get => _targetPosition;
        set
        {
            _targetPosition.x = Mathf.RoundToInt(value.x);
            _targetPosition.y = Mathf.RoundToInt(value.y);
        }
    }

    /// <summary>
    /// Set direction and Update the animation
    /// </summary>
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
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _ghostAI = GetComponent<GhostAI>();
    }

    void SetMovementAnimation(Vector2 direction)
    {
        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
    }

    public void MoveToTargetPosition()
    {
        Vector2 currentPosition = transform.position;
        Vector2 position = Vector2.MoveTowards(currentPosition, _targetPosition, GetSpeedBasedOnGhostState());
        _rigidbody2D.MovePosition(position);
    }

    public bool HasReachTarget()
    {
        Vector2 currentPosition = transform.position;
        return Vector3.Distance(currentPosition, _targetPosition) < _smallNumber;
    }

    /// <summary>
    /// Update animation Frightened and schedule the blinking ending of that state
    /// </summary>
    /// <param name="isFrightened"></param>
    public void Frightened(bool isFrightened)
    {
        _animator.SetBool("Frightened", isFrightened);

        if (isFrightened)
        {
            StartCoroutine(UnityUtils.DelayedAction(GetEndindFrightenedTime(), EndingFrightened));

        }
        else
        {
            _animator.SetBool("EndingFrightend", false);

        }
    }

    public void EndingFrightened()
    {
        _animator.SetBool("EndingFrightend", true);

    }

    float GetEndindFrightenedTime()
    {
        return _ghostAI.FrightenedTime * 0.75f;
    }

    public void Dead(bool isDead)
    {
        _animator.SetBool("Dead", isDead);
    }

    /// <summary>
    /// Initial movement of the ghost when they are in the GhostHouse
    /// </summary>
    public void VerticalTilt()
    {
        Vector2 currentPos = transform.position;
        Vector2 tiltPos = _startPos;
        tiltPos.y += Mathf.PingPong(Time.time, 1);
        Vector2 position = Vector2.MoveTowards(currentPos, tiltPos, GetSpeedBasedOnGhostState());
        _rigidbody2D.MovePosition(position);

    }


    public void ResetPosition()
    {
        _targetPosition = _startPos;
        transform.position = _startPos;
    }

    /// <summary>
    /// The speed change on some states, to improve gamefeel,
    /// When dead, it speedup to reach the ghosthouse faster
    /// When Frightened, it slow down to give a chance to be eaten by pacman
    /// </summary>
    /// <returns></returns>
    float GetSpeedBasedOnGhostState()
    {
        if (_ghostAI.CurrentState == GhostAI.GhostState.Dead)
            return _baseSpeed * 2f;
        if (_ghostAI.CurrentState == GhostAI.GhostState.Frightened)
            return _baseSpeed * 0.5f;
        return _baseSpeed;
    }
}