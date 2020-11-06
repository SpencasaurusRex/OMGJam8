using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform PointA;
    public Transform PointB;

    Collider2D[] colliderResults;
    int mask;
    
    void Start()
    {
        colliderResults = new Collider2D[1];
        mask = LayerMask.GetMask("Player");
    }

    void Update()
    {
        Vector2 center = (PointA.position + PointB.position) * 0.5f;
        Vector2 size = PointA.position - PointB.position;
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);

        if (Physics2D.OverlapBoxNonAlloc(center, size, 0, colliderResults, mask) > 0)
        {
            colliderResults[0].GetComponent<PlayerController>().Die();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(.8f, 0.3f, 0.3f, 0.5f);
        if (PointA && PointB)
        {
            Vector3 center = (PointA.position + PointB.position) * 0.5f;
            Vector3 size = PointA.position - PointB.position;
        
            Gizmos.DrawCube(center, size);    
        }
    }
}
