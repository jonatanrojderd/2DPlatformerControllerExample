using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float GravityModifier = 1f;
    public float MinimumMoveDistance = 0.001f;
    public float MinimumGroundNormalY = 0.65f;
    
    public bool IsGrounded;
    
    private Vector2 _groundNormal;
    
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);
    
    private Rigidbody2D _rigidbody;
    private float _shellRadius = 0.01f;
    
    protected Vector2 Velocity;
    protected Vector2 TargetVelocity;

    private void Start()
    {
        _contactFilter.useTriggers = false;

        int collisionMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
        _contactFilter.SetLayerMask(collisionMask);

        _contactFilter.useLayerMask = true;
    }
    
    private void OnEnable()
    {
        if (_rigidbody == false)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        TargetVelocity = Vector2.zero;
        ComputeVelocity();
    }
    
    private void FixedUpdate()
    {
        Velocity += Physics2D.gravity * (GravityModifier * Time.deltaTime);
        Velocity.x = TargetVelocity.x;

        IsGrounded = false;
        
        Vector2 deltaPosition = Velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition;
        Movement(move, false);
        
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }
    
    private void Movement(Vector2 direction, bool yMovement)
    {
        float distance = direction.magnitude;
        if (distance > MinimumMoveDistance)
        {
            int count = _rigidbody.Cast(direction, _contactFilter, _hitBuffer, distance + _shellRadius);

            _hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i < _hitBufferList.Count; i++)
            {
                Vector2 currentNormal = _hitBufferList[i].normal;
                if (currentNormal.y > MinimumGroundNormalY)
                {
                    IsGrounded = true;
                    if (yMovement)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(Velocity, currentNormal);
                if (projection < 0)
                {
                    Velocity -= projection * currentNormal;
                }

                float modifiedDistance = _hitBufferList[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        _rigidbody.position += direction.normalized * distance;
    }

    protected virtual void ComputeVelocity()
    {
        // A virtual method can have an implementation of code,
        // but can be overridden by another class that's inheriting from it.
    }
    
    private void Movement_(Vector2 direction, bool yMovement)
    {
        float distance = direction.magnitude;
        if(distance > MinimumMoveDistance)
        {
            int count = _rigidbody.Cast(direction, _contactFilter, _hitBuffer, distance + _shellRadius);
            print(count);
            
            _hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (var i = 0; i < _hitBufferList.Count; i++)
            {
                Vector2 currentNormal = _hitBufferList[i].normal;
                if (currentNormal.y > MinimumGroundNormalY)
                {
                    IsGrounded = true;
                    if (yMovement)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(Velocity, currentNormal);
                if (projection < 0)
                {
                    Velocity -= projection * currentNormal;
                }

                // float modifiedDistance = raycastHit.distance - _shellRadius;
                float modifiedDistance = _hitBufferList[i].distance - distance;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        
        _rigidbody.position += direction.normalized * direction;
    }
}