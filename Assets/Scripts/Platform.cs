using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool Collapses;
    public float CollapseTime;
    public float JitterAmount = 0.1f;
    public float JitterSpeed = 10f;
    [Range(1, MaxSize)]
    public int Size;
    public GameObject PlatformChildPrefab;

    const int MaxSize = 8;
    public Sprite[] Sprites;
    
    SpriteRenderer[] children;
    bool collapsing;
    bool collapsed;
    float timeSinceCollapse;
    Vector2 pos;
    
    int playerMask;
    Collider2D coll;

    Sprite GetSprite(int i)
    {
        int c = (Collapses ? 4 : 0) + (collapsing ? 4 : 0);
        int single = c;
        int left = c + 1;
        int center = c + 2;
        int right = c + 3;

        if (Size == 1)
        {
            return Sprites[single];
        }
        if (i == 0)
        {
            return Sprites[left];
        }
        if (i == Size - 1)
        {
            return Sprites[right];
        }
        return Sprites[center];
    }
    
    void OnValidate()
    {
        if (children == null || children.Length != MaxSize)
        {
            children = new SpriteRenderer[MaxSize];
        }
        for (int i = 0; i < MaxSize; i++)
        {
            children[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
        
        
        for (int i = 0; i < Size; i++)
        {
            Vector3 position = Vector3.zero;
            position.x += i - (Size - 1) * 0.5f;
            
            children[i].transform.localPosition = position;
            children[i].sprite = GetSprite(i);
            children[i].enabled = true;
        }

        for (int i = Size; i < MaxSize; i++)
        {
            children[i].enabled = false;
        }
        
        var bc = GetComponent<BoxCollider2D>();
        var s = bc.size;
        s.x = Size;
        bc.size = s;
    }

    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
        coll = GetComponent<Collider2D>();

        
        children = new SpriteRenderer[MaxSize];
        for (int i = 0; i < MaxSize; i++)
        {
            children[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
        
        GameController.OnFadeoutComplete += FadeOut;
        pos = transform.position;
    }

    void FadeOut(GameController.FadeContext context)
    {
        Respawn();
    }
    
    void OnDestroy()
    {
        GameController.OnFadeoutComplete -= FadeOut;
    }

    void Respawn()
    {
        collapsed = collapsing = false;
        timeSinceCollapse = 0;
        coll.enabled = true;
        for (int i = 0; i < Size; i++)
        {
            children[i].enabled = true;
            children[i].sprite = GetSprite(i);
            children[i].gameObject.transform.localPosition = new Vector2(i -(Size - 1) * 0.5f, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;
        if (Collapses && !collapsing) 
        {
            collapsing = true;
            timeSinceCollapse = 0;

            for (int i = 0; i < Size; i++)
            {
                children[i].sprite = GetSprite(i);
            }
        }
    }

    void Update()
    {
        if (collapsing)
        {
            // Make children platforms jitter
            for (int i = 0; i < Size; i++)
            {
                float t = (timeSinceCollapse - CollapseTime);
                float y = collapsed ? -t * t * 15 : 0;
                var pos = new Vector2(-(Size - 1) * 0.5f + i, y);
                if (!collapsed)
                    pos += new Vector2(
                        Mathf.Cos(2 * JitterSpeed * Time.time), 
                        Mathf.Sin(JitterSpeed * Time.time)
                    ) * JitterAmount;
                
                children[i].gameObject.transform.localPosition = pos;
            }

            timeSinceCollapse += Time.deltaTime;
            if (timeSinceCollapse >= CollapseTime)
            {
                collapsed = true;
                coll.enabled = false;
            }
        }
        else transform.position = pos;
    }
}

