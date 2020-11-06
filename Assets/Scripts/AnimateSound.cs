using UnityEngine;

public class AnimateSound : MonoBehaviour
{
    public AudioClip Clip;
    public float Volume;
    
    AudioSource source;
        
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        source.PlayOneShot(Clip, Volume);
    }
}