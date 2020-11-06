using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public float Speed = 5;
    public float JumpVelocity = 10;
    public Transform Feet;
    public float CoyoteTime = 0.2f;
    
    float gravity;
    Rigidbody2D rb;
    bool grounded;
    Collider2D[] groundDetection;
    int groundMask;
    float lastTouchingGround;
    float lastJump;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = new Collider2D[4];
        groundMask = LayerMask.GetMask("Ground");
        gravity = rb.gravityScale;
    }

    void Update()
    {
        grounded = Physics2D.OverlapBoxNonAlloc(Feet.position, Vector2.one * 0.05f, 0, groundDetection, groundMask) > 0;
        if (grounded)
        {
            lastTouchingGround = 0;
        }
        else
        {
            lastTouchingGround += Time.deltaTime;
        }
        
        var targetVel = rb.velocity;
        targetVel.x = Input.GetAxisRaw("Horizontal") * Speed;
        
        if (lastTouchingGround <= CoyoteTime && Input.GetKey(KeyCode.Space))
        {
            lastTouchingGround += CoyoteTime + 0.01f; // Disqualify us from jumping next frame
            targetVel.y = JumpVelocity;
        }
        
        if (targetVel.y < 0)
        {
            rb.gravityScale = gravity * 1.5f;
            
        }
        if (targetVel.y >= 0)
        {
            rb.gravityScale = gravity;
        }
        
        rb.velocity = targetVel;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(Feet.position, Vector3.one * 0.1f);
    }
}
