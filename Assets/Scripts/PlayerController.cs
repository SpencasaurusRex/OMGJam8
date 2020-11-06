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
    int stateIndex = Animator.StringToHash("State");
    
    bool tryJump;
    Vector2 targetVel;

    enum AnimState
    {
        Idle = 0,
        Running = 1,
        Jumping = 2,
        Falling = 3
    }
    
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
        AnimState state = AnimState.Idle;
        
        targetVel = rb.velocity;
        targetVel.x = Input.GetAxisRaw("Horizontal") * Speed;
        
        if (Mathf.Abs(targetVel.x) > 0)
        {
            state = AnimState.Running;
        }
        
        if (targetVel.x > 0) transform.localScale = new Vector3(1, 1, 1);
        if (targetVel.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (Input.GetKey(KeyCode.Space))
        {
            tryJump = true;
        }

        if (rb.velocity.y < -0.05f)
        {
            state = AnimState.Falling;
        }

        if (rb.velocity.y > 0.05f)
        {
            state = AnimState.Jumping;
        }
        
        anim.SetInteger(stateIndex, (int)state);
    }

    float lastHeight;
    bool shownThisJump;
    
    void FixedUpdate()
    {
        if (lastHeight > transform.position.y && !shownThisJump)
        {
            print(lastHeight);
            shownThisJump = true;
        }
        lastHeight = transform.position.y;
        
        grounded = Physics2D.OverlapBoxNonAlloc(Feet.position, Vector2.one * 0.05f, 0, groundDetection, groundMask) > 0;
        if (grounded)
        {
            lastTouchingGround = 0;
        }
        else
        {
            lastTouchingGround += Time.fixedDeltaTime;
        }
        
        if (lastTouchingGround <= CoyoteTime && tryJump)
        {
            shownThisJump = false;
            lastTouchingGround += CoyoteTime + 0.01f; // Disqualify us from jumping next frame
            targetVel.y = JumpVelocity;
        }
        
        if (targetVel.y > 0.05f && tryJump)
        {
            rb.gravityScale = gravity;
        }
        else
        {
            rb.gravityScale = gravity * 2f;
        }
        
        tryJump = false;
        
        rb.velocity = targetVel;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(Feet.position, Vector3.one * 0.1f);
    }
}
