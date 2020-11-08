using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float JumpVelocity = 10;
    public Transform Feet;
    public float CoyoteTime = 0.2f;
    public Vector2 RespawnVelocity;
    public float RespawnControlLockTime = 0.5f;
    public Transform Target;
    public OneShotSound JumpSound;
    public OneShotSound LandingSound;
    public float Acceleration = 10;
    
    public float MaxSpeed = 5;
    
    bool alive = true;
    
    public RespawnMushroom LastMushroom;
    
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
    Collider2D coll;
    
    bool tryJump;
    Vector2 targetVel;
    
    bool touchingPlatform;
    bool controlsLocked;
    
    enum AnimState
    {
        Idle = 0,
        Running = 1,
        Jumping = 2,
        Falling = 3,
        Dead = 4
    }
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        
        detectionResults = new Collider2D[4];
        groundMask = LayerMask.GetMask("Ground", "Platform");
        mushroomMask = LayerMask.GetMask("Mushroom");
        gravity = rb.gravityScale;
        
        GameController.OnFadeoutComplete += FadeOutComplete;
        GameController.OnFadeInComplete += FadeInComplete;

        transform.position = LastMushroom.transform.position;
        LastMushroom.Respawn();
    }

    void OnDestroy()
    {
        GameController.OnFadeoutComplete -= FadeOutComplete;
        GameController.OnFadeInComplete -= FadeInComplete;
    }

    void FadeOutComplete(GameController.FadeContext context)
    {
        if (context == GameController.FadeContext.Respawn)
        {
            alive = true;
            
            // Teleport to mushroom
            var tfp = LastMushroom.transform.position;
            var mushroomPosition = new Vector3(tfp.x, tfp.y, transform.position.z);
            transform.position = mushroomPosition;
            
            controlsLocked = true;
            targetVel = rb.velocity = Vector2.zero;

            LastMushroom.Respawn();
        
            GameController.instance.StartFadeIn(GameController.FadeContext.Respawn);
            anim.SetInteger(stateIndex, (int)AnimState.Idle);
        }
    }

    void FadeInComplete(GameController.FadeContext context)
    {
        LastMushroom.Pop();
        controlsLocked = true;
            
        // Apply force to player
        targetVel = RespawnVelocity;
        targetVel.x *= LastMushroom.Direction;
        rb.velocity = targetVel;
        StartCoroutine(UnlockControls());
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
            targetVel.x = 0;
            targetVel.x += Input.GetKey(KeyCode.A) ? -1 : 0;
            targetVel.x += Input.GetKey(KeyCode.D) ? 1 : 0;
            targetVel.x *= MaxSpeed;
            
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
        
        if (rb.velocity.y < -0.05f && !touchingPlatform)
        {
            state = AnimState.Falling;
        }

        if (rb.velocity.y > 0.05f)
        {
            state = AnimState.Jumping;
        }

        if (anim.GetInteger(stateIndex) != (int) AnimState.Dead)
        {
            anim.SetInteger(stateIndex, (int)state);
        }

        if (Input.GetKey(KeyCode.E) && lastDoor != null)
        {
            sceneLoad = lastDoor.SceneBuildIndex;
            GameController.instance.StartFadeOut(GameController.FadeContext.LevelChange);
            GameController.OnFadeoutComplete += LoadScene;
        }
    }

    int sceneLoad;
    
    void LoadScene(GameController.FadeContext context)
    {
        if (context != GameController.FadeContext.LevelChange) return;
        SceneManager.LoadScene(sceneLoad);
        GameController.OnFadeoutComplete -= LoadScene;
    }
    
    void FixedUpdate()
    {
        if (Physics2D.OverlapBoxNonAlloc(transform.position, Vector2.one * 0.5f, 0, detectionResults, mushroomMask) > 0)
        {
            var lm = detectionResults[0].GetComponent<RespawnMushroom>();
            if (lm != LastMushroom)
            {
                LastMushroom = lm;
                lm.Mark();
            }
        }
        
        bool lastGrounded = grounded;
        grounded = Physics2D.OverlapBoxNonAlloc(Feet.position, Vector2.one * 0.05f, 0, detectionResults, groundMask) > 0;
        if (grounded)
        {
            touchingPlatform = detectionResults[0].gameObject.layer == LayerMask.NameToLayer("Platform");
            lastTouchingGround = 0;
        }
        else
        {
            lastTouchingGround += Time.fixedDeltaTime;
        }

        if (grounded && !lastGrounded)
        {
            Instantiate(LandingSound, transform);
        }
        
        if (lastTouchingGround <= CoyoteTime && tryJump)
        {
            Instantiate(JumpSound, transform);
            
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

        if (Mathf.Abs(targetVel.x - rb.velocity.x) > Acceleration * Time.fixedDeltaTime)
        {
            float deltaDir = Mathf.Sign(targetVel.x - rb.velocity.x);

            float accelBonus = deltaDir == Mathf.Sign(-rb.velocity.x) ? 2f : 1;
            float targetXVel = Mathf.Clamp(deltaDir * Acceleration * accelBonus * Time.fixedDeltaTime + rb.velocity.x, -MaxSpeed, MaxSpeed);
            targetVel.x = targetXVel;    
        }
        
        rb.velocity = targetVel;
    }

    public void Bounce(float amount)
    {
        rb.velocity = new Vector2(rb.velocity.x, amount);
        lastTouchingGround = 0;
    }

    public void Die(bool animate)
    {
        if (!alive) return;
        alive = false;

        if (animate)
        {
            anim.SetInteger(stateIndex, (int)AnimState.Dead);
        }
            
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

    Door lastDoor;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            Die(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Door"))
        {
            lastDoor = other.gameObject.GetComponent<Door>();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Door"))
        {
            lastDoor = null;
        }
    }
}
