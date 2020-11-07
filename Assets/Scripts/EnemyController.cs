using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    enum AnimState
    {
        Walk = 0,
        Die = 1
    }
    
    Vector2 initialPosition; 
    Collider2D coll;
    
    public float BounceAmount = 10;
    public float Speed = 2.5f;
    Collider2D[] collisionResults;
    int groundMask;
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer sr;
    
    bool dead;
    float initialSpeed;
    Collider2D playerCollider;
    
    TweenerCore<Color, Color, ColorOptions> fadeTween;

    void Start()
    {
        initialPosition = transform.position;
        collisionResults = new Collider2D[1];
        groundMask = LayerMask.GetMask("Ground");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        GameController.OnFadeoutComplete += _ => Respawn();
        coll = GetComponent<Collider2D>();
        initialSpeed = Speed;
        sr = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        rb.velocity = new Vector2(Speed, rb.velocity.y);
        
        Vector2 position = new Vector2(5f/16 * Mathf.Sign(Speed), -0.3f) + (Vector2)transform.position;
        // Check for wall collision
        if (Physics2D.OverlapBoxNonAlloc(position, Vector2.one * 0.05f, 0, collisionResults, groundMask) > 0)
        {
            Speed = -Speed;
            return;
        }
        
        position = new Vector2(6f/16 * Mathf.Sign(Speed), -0.5f) + (Vector2)transform.position;
        // Check for ground in front of us
        if (Physics2D.OverlapBoxNonAlloc(position, Vector2.one * 0.05f, 0, collisionResults, groundMask) == 0) 
        {
            Speed = -Speed;
        }
    }

    public void Die()
    {
        print("Die");
        animator.SetInteger("State", (int)AnimState.Die);
        Speed = 0;
        dead = true;
        fadeTween = sr.DOFade(0, 1f).SetEase(Ease.InCubic);
    }
    
    public void Respawn()
    {
        transform.position = initialPosition;
        print("Walk");
        animator.SetInteger("State", (int)AnimState.Walk);
        dead = false;
        Speed = initialSpeed;
        fadeTween?.Complete();
        sr.color = Color.white;
        if (playerCollider != null)
            Physics2D.IgnoreCollision(coll, playerCollider, false);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (dead) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerController>(); 
            if (other.rigidbody.velocity.y < -0.01f)
            {
                player.Bounce(BounceAmount);
                Die();
            }
            else
            {
                player.Die(true);
            }
            
            Physics2D.IgnoreCollision(coll, other.collider);
            playerCollider = other.collider;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Vector2 position = new Vector2(5f/16 * Mathf.Sign(Speed), -0.3f) + (Vector2)transform.position;
        Gizmos.DrawCube(position, Vector3.one * 0.05f);
        
        position = new Vector2(6f/16 * Mathf.Sign(Speed), -0.5f) + (Vector2)transform.position;
        Gizmos.DrawCube(position, Vector3.one * 0.05f);
    }
}
