using System;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    public float MaxSpeed = 7f;
    public float JumpTakeOffSpeed = 7f;

    [SerializeField]
    private Vector2 _facingDirection = Vector2.right;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            Velocity.y = JumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (Velocity.y > 0)
            {
                Velocity.y *= 0.5f;
            }
        }

        if (move.x > 0 && _facingDirection != Vector2.right)
        {
            _spriteRenderer.flipX = false;
            _facingDirection = Vector2.right;
        }
        else if (move.x < 0 && _facingDirection != Vector2.left)
        {
            _spriteRenderer.flipX = true;
            _facingDirection = Vector2.left;
        }

        _animator.SetBool("grounded", IsGrounded);
        _animator.SetFloat("velocityX", Math.Abs(move.x) / MaxSpeed);

        TargetVelocity = move * MaxSpeed;
    }
}