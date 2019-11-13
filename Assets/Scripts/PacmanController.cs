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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.SetScore(GetValueFromTriggerCollision(collision));
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


}
