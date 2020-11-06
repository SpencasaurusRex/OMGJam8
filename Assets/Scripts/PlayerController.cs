using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public float Speed = 5;
    public float JumpVelocity = 10;
    public Transform Feet;
    public float CoyoteTime = 0.2f;
    
    Animator anim;
    float gravity;
    Rigidbody2D rb;
    bool grounded;
    Collider2D[] groundDetection;
    int groundMask;
    float lastTouchingGround;
    float lastJump;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        groundDetection = new Collider2D[4];
        groundMask = LayerMask.GetMask("Ground");
        gravity = rb.gravityScale;
    }

    void Update()
    {
        int state = 0;
        
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

        if (Mathf.Abs(targetVel.x) > 0)
        {
            state = 1;
            print(state);
        }
        
        if (targetVel.x > 0) transform.localScale = new Vector3(1, 1, 1);
        if (targetVel.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        
        if (lastTouchingGround <= CoyoteTime && Input.GetKey(KeyCode.Space))
        {
            lastTouchingGround += CoyoteTime + 0.01f; // Disqualify us from jumping next frame
            targetVel.y = JumpVelocity;
        }
        
        if (targetVel.y < -0.05f)
        {
            rb.gravityScale = gravity * 1.5f;
            state = 3;
        }
        if (targetVel.y > 0.05f)
        {
            rb.gravityScale = gravity;
            state = 2;
        }
        
        rb.velocity = targetVel;

        anim.SetInteger("State", state);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(Feet.position, Vector3.one * 0.1f);
    }
}
