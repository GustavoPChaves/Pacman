﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CircleCollider2D))]
public class PacmanController : MonoBehaviour
{

    [SerializeField]
    float _speed = 1;
    [SerializeField]
    Vector2 _startPosition;

    Vector2 _direction;
    Animator _animator;
    Rigidbody2D _rigidbody2D;

    public Vector2 Direction { get => _direction; private set => _direction = value; }

    bool hasDied;

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
        if (hasDied) return;
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");

    }

    private void FixedUpdate()
    {
        if (IsValidDirection(_direction))
        {
            SetRigidbodyVelocity(_direction, _speed);
            SetMovementAnimation(_direction);
        }
    }

    void SetRigidbodyVelocity(Vector2 direction, float speed)
    {
        if (!direction.IsVectorZero())
            _rigidbody2D.velocity = speed * direction.normalized;
    }

    void SetMovementAnimation(Vector2 direction)
    {
        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
    }

    bool IsValidDirection(Vector2 direction)
    {
        // cast line from next to pacman to checkif it can keep going
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.5f, direction.y * 0.5f);
        RaycastHit2D hit = Physics2D.BoxCast(pos, Vector2.one * 0.8f, 0, direction, 1f, 1 << 8);
        return hit.collider == null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost"))
        {
            return;
        }
        GameManager.Instance.SetScore(GetValueFromTriggerCollision(collision), collision.gameObject);
        collision.gameObject.SetActive(false);
        PlayPacmanSound(collision.tag);
    }

    
    void PlayPacmanSound(string type)
    {
        if (type.Equals("Pellet"))
        {
            AudioManager.Instance.Play(AudioClipType.chomp);
        }
        else if (type.Equals("Fruits"))
        {
            AudioManager.Instance.Play(AudioClipType.fruit);
        }
        else if (type.Equals("Ghost"))
        {
            AudioManager.Instance.Play(AudioClipType.blueGhost);
        }
    }

    int GetValueFromTriggerCollision(Collider2D collision)
    {
        var collectible = collision.GetComponent<Collectible>();
        return collectible != null ? collectible.PointValue : 0;
    }

    public void Die()
    {
        _animator.SetBool("Die", true);
        hasDied = true;
        AudioManager.Instance.Play(AudioClipType.death);
    }

    public void Reset()
    {
        _animator.SetBool("Die", false);
        hasDied = false;
        transform.position = _startPosition;
        _direction = Vector2.right;
        SetMovementAnimation(_direction);
        SetRigidbodyVelocity(_direction, _speed);
    }


}
