using DG.Tweening;
using UnityEngine;

public class Sign : MonoBehaviour
{
    public RectTransform Canvas;
    public float TargetScale = 0.01f;
    public float AnimDuration = 0.4f;
    
    int playerMask;
    
    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if ((other.gameObject.layer & playerMask) > 0)
        {
            print("In");

            DOTween.To(() => Canvas.localScale.x, (float x) =>
            {
                var ls = Canvas.localScale;
                ls.x = x;
                Canvas.localScale = ls;
            }, TargetScale, AnimDuration).SetEase(Ease.OutBack);
            DOTween.To(() => Canvas.localScale.y, (float y) =>
            {
                var ls = Canvas.localScale;
                ls.y = y;
                Canvas.localScale = ls;
            }, TargetScale, AnimDuration-0.1f).SetEase(Ease.OutBack);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        print("Out");
        //if ((other.gameObject.layer & playerMask) > 0)
        {
            DOTween.To(() => Canvas.localScale.x, (float x) =>
            {
                var ls = Canvas.localScale;
                ls.x = x;
                Canvas.localScale = ls;
            }, 0, AnimDuration).SetEase(Ease.InQuad);
            DOTween.To(() => Canvas.localScale.y, (float y) =>
            {
                var ls = Canvas.localScale;
                ls.y = y;
                Canvas.localScale = ls;
            }, 0, AnimDuration-0.1f).SetEase(Ease.InQuad);
        }        
    }
}
