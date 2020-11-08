using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CustomEnemy : MonoBehaviour
{
    public float WaitTime;
    float timeWaited;
    bool waiting;
    bool triggered;
    
    public float TargetScale = 0.01f;
    public float AnimDuration = 0.4f;
    public RectTransform Canvas;
    public Transform Platform;

    TweenerCore<float, float, FloatOptions> canvasXTween;
    TweenerCore<float, float, FloatOptions> canvasYTween;
    
    bool killed;
    
    public void Kill()
    {
        print("Kill");
        canvasXTween.Kill();
        canvasYTween.Kill();
        Canvas.localScale = Vector2.zero;
        killed = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (killed) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            waiting = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (killed) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            waiting = false;
            timeWaited = 0;
        }
    }

    void Update()
    {
        if (killed) return;
        if (triggered) return;
        if (waiting) 
            timeWaited += Time.deltaTime;

        if (timeWaited > WaitTime)
        {
            triggered = true;
            canvasXTween = DOTween.To(() => Canvas.localScale.x, x =>
            {
                var ls = Canvas.localScale;
                ls.x = x;
                Canvas.localScale = ls;
            }, TargetScale, AnimDuration).SetEase(Ease.OutBack);
            canvasYTween = DOTween.To(() => Canvas.localScale.y, y =>
            {
                var ls = Canvas.localScale;
                ls.y = y;
                Canvas.localScale = ls;
            }, TargetScale, AnimDuration-0.1f).SetEase(Ease.OutBack);
            
            DOTween.To(() => Platform.transform.position.y, value =>
            {
                Platform.transform.position = new Vector2(Platform.transform.position.x, value);
            }, Platform.transform.position.y + 1, 1f).SetEase(Ease.Linear);
            
            gameObject.transform.parent.GetComponent<Collider2D>().enabled = false;
            gameObject.transform.parent.GetComponent<Rigidbody2D>().gravityScale = 0f;
        }
    }
}
