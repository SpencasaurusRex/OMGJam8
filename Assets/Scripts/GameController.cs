using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    
    public Material CircleFadeMaterial;
    public bool PlayMusic;
    
    float size;
    public float MaxSize = 2.5f;
    public float FadeTime = 0.5f;
    
    float fadeoutTime;

    public delegate void FadeOutComplete(FadeContext context);
    public static event FadeOutComplete OnFadeoutComplete;

    public delegate void FadeInComplete(FadeContext context);
    public static event FadeInComplete OnFadeInComplete;


    public enum FadeContext
    {
        Respawn,
        LevelChange
    }
    
    FadeContext context;
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartFadeIn(FadeContext.LevelChange);
        Screen.fullScreen = false;
        //Screen.fullScreenMode = FullScreenMode.MaximizedWindow;

        if (PlayMusic)
        {
            var music = GameObject.Find("Music");
            if (music)
            {
                var audio = music.GetComponent<AudioSource>();
                if (!audio.isPlaying)
                {
                    audio.Play();
                }
            }
        }
    }
    
    public void StartFadeOut(FadeContext context)
    {
        fadeoutTime = 0;
        this.context = context;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float progress = 0;
        while (progress < 1)
        {
            fadeoutTime += Time.deltaTime;
            progress = Mathf.InverseLerp(0, FadeTime, fadeoutTime);
            CircleFadeMaterial.SetFloat("Vector1_9F8BFC69", Mathf.Lerp(MaxSize, 0, progress));
            yield return new WaitForEndOfFrame();
        }
        OnFadeoutComplete?.Invoke(context);

        yield return null;
    }

    public void StartFadeIn(FadeContext context)
    {
        this.context = context;
        fadeoutTime = 0;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float progress = 0;

        while (progress < 1)
        {
            fadeoutTime += Time.deltaTime;
            progress = Mathf.InverseLerp(0, FadeTime, fadeoutTime);
            CircleFadeMaterial.SetFloat("Vector1_9F8BFC69", Mathf.Lerp(0, MaxSize, progress));
            yield return new WaitForEndOfFrame();
        }

        OnFadeInComplete?.Invoke(context);
        yield return null;
    }
    
}