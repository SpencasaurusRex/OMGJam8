using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float JumpVelocity = 10;
    public Transform Feet;
    public float CoyoteTime = 0.2f;
    public Vector2 RespawnVelocity;
    public float RespawnControlLockTime = 0.5f;
    public Transform Target;
    
    bool alive = true;
    
    RespawnMushroom lastMushroom;
    
    Animator anim;
    float gravity;
    Rigidbody2D rb;
    bool grounded;
    Collider2D[] detectionResults;
    int groundMask;
    int mushroomMask;
    float lastTouchingGround;
    float lastJump;
    int stateIndex = Animator.StringToHash("State");
    Collider2D collider;
    
    bool tryJump;
    Vector2 targetVel;
    
    bool controlsLocked;
    
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
        collider = GetComponent<BoxCollider2D>();
        
        detectionResults = new Collider2D[4];
        groundMask = LayerMask.GetMask("Ground");
        mushroomMask = LayerMask.GetMask("Mushroom");
        gravity = rb.gravityScale;
        
        GameController.OnFadeoutComplete += FadeOutComplete;
        GameController.OnFadeIncomplete += FadeInComplete;
    }

    void FadeOutComplete(GameController.FadeContext context)
    {
        if (context == GameController.FadeContext.Respawn)
        {
            alive = true;
            
            // Teleport to mushroom
            var tfp = lastMushroom.transform.position;
            var mushroomPosition = new Vector3(tfp.x, tfp.y, transform.position.z);
            transform.position = mushroomPosition;
            
            controlsLocked = true;
            targetVel = rb.velocity = Vector2.zero;

            lastMushroom.Respawn();
        
            GameController.instance.StartFadeIn(GameController.FadeContext.Respawn);
        }
    }

    void FadeInComplete(GameController.FadeContext context)
    {
        if (context == GameController.FadeContext.Respawn)
        {
            lastMushroom.Pop();
            controlsLocked = true;
            
            // Apply force to player
            targetVel = rb.velocity = RespawnVelocity;
            StartCoroutine(UnlockControls());
        }
    }

    IEnumerator UnlockControls()
    {
        yield return new WaitForSeconds(RespawnControlLockTime);
        controlsLocked = false;
    }

    void Update()
    {
        targetVel = rb.velocity;

        if (!controlsLocked)
        {
            targetVel.x = Input.GetAxisRaw("Horizontal") * Speed;
            
            if (Input.GetKey(KeyCode.Space))
            {
                tryJump = true;
            }
        }
        
        AnimState state = AnimState.Idle;
        
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            state = AnimState.Running;
        }
        
        if (rb.velocity.x > 0) transform.localScale = new Vector3(1, 1, 1);
        if (rb.velocity.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        
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

    void FixedUpdate()
    {
        if (Physics2D.OverlapBoxNonAlloc(transform.position, Vector2.one * 0.5f, 0, detectionResults, mushroomMask) > 0)
        {
            lastMushroom = detectionResults[0].GetComponent<RespawnMushroom>();
        }
        
        grounded = Physics2D.OverlapBoxNonAlloc(Feet.position, Vector2.one * 0.05f, 0, detectionResults, groundMask) > 0;
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

    public void Die()
    {
        if (!alive) return;
        alive = false;
        GameController.instance.StartFadeOut(GameController.FadeContext.Respawn);
        controlsLocked = true;
        
        var vel = rb.velocity;
        vel.x = 0;
        rb.velocity = vel;
        
        tryJump = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(Feet.position, Vector3.one * 0.1f);
    }
}
