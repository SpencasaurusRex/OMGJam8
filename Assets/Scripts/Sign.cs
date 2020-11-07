using DG.Tweening;
using UnityEngine;

public class Sign : MonoBehaviour
{
    public RectTransform Canvas;
    public float TargetScale = 0.01f;
    public float AnimDuration = 0.4f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            DOTween.To(() => Canvas.localScale.x, x =>
            {
                var ls = Canvas.localScale;
                ls.x = x;
                Canvas.localScale = ls;
            }, TargetScale, AnimDuration).SetEase(Ease.OutBack);
            DOTween.To(() => Canvas.localScale.y, y =>
            {
                var ls = Canvas.localScale;
                ls.y = y;
                Canvas.localScale = ls;
            }, TargetScale, AnimDuration-0.1f).SetEase(Ease.OutBack);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            DOTween.To(() => Canvas.localScale.x, x =>
            {
                var ls = Canvas.localScale;
                ls.x = x;
                Canvas.localScale = ls;
            }, 0, AnimDuration).SetEase(Ease.InQuad);
            DOTween.To(() => Canvas.localScale.y, y =>
            {
                var ls = Canvas.localScale;
                ls.y = y;
                Canvas.localScale = ls;
            }, 0, AnimDuration).SetEase(Ease.InQuad);
        }        
    }
}
