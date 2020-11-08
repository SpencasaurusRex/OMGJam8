using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("")]
    public bool X;
    public Ease EaseX;
    public Ease EaseXBack;
    public float StartX;
    public float EndX;
    public float TimeX;
    public float WaitX;
    public bool ReverseX;
    
    [Header("")]
    public bool Y;
    public Ease EaseY;
    public Ease EaseYBack;
    public float StartY;
    public float EndY;
    public float TimeY;
    public float WaitY;
    public bool ReverseY;
    
    TweenerCore<float, float, FloatOptions> xTween;
    TweenerCore<float, float, FloatOptions> yTween;
    
    void Start()
    {
        if (X)
        {
            xTween = XTween(true).OnComplete(() => StartCoroutine(LoopX(false)));
        }

        if (Y) 
            yTween = YTween(true).OnComplete(() => StartCoroutine(LoopY(false)));
    }

    IEnumerator LoopX(bool forward)
    {
        yield return new WaitForSeconds(WaitX);
        xTween = XTween(forward);
        xTween.OnComplete(() => StartCoroutine(LoopX(!forward)));
    }
    
    TweenerCore<float, float, FloatOptions> XTween(bool forward)
    {
        float endX = forward || !ReverseX ? EndX : StartX;
        
        return DOTween.To(() => transform.position.x, value =>
        {
            transform.position = new Vector2(value, transform.position.y);
        }, endX, TimeX).SetEase(forward ? EaseX : EaseXBack);
    }

    IEnumerator LoopY(bool forward)
    {
        yield return new WaitForSeconds(WaitY);
        yTween = YTween(forward);
        yTween.OnComplete(() => StartCoroutine(LoopY(!forward)));
    }
    
    TweenerCore<float, float, FloatOptions> YTween(bool forward)
    {
        float endY = forward || !ReverseY ? EndY : StartY;
        return DOTween.To(() => transform.position.y, value =>
        {
            transform.position = new Vector2(transform.position.x, value);
        }, endY, TimeY).SetEase(forward ? EaseY : EaseYBack);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        
        float sx = X ? StartX : transform.position.x;
        float ex = X ? EndX : transform.position.x;
        float sy = Y ? StartY -0.5f : transform.position.y -0.5f;
        float ey = Y ? EndY -0.5f: transform.position.y -0.5f;
        
        Vector3 a = new Vector3(sx, sy);
        Vector3 b = new Vector3(sx, ey);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawCube(a, Vector3.one * 0.5f);
        Gizmos.DrawCube(b, Vector3.one * 0.5f);
        
        a = new Vector3(ex, sy);
        b = new Vector3(ex, ey);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawCube(a, Vector3.one * 0.5f);
        Gizmos.DrawCube(b, Vector3.one * 0.5f);
        
        a = new Vector3(sx, sy);
        b = new Vector3(ex, sy);
        Gizmos.DrawLine(a, b);
        
        a = new Vector3(sx, ey);
        b = new Vector3(ex, ey);
        Gizmos.DrawLine(a, b);
    }
}
