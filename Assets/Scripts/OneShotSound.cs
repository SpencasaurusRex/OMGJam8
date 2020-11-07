using UnityEngine;

public class OneShotSound : MonoBehaviour
{
    public float Volume;
    
    AudioSource source;
    
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = Volume;
        Destroy(gameObject, source.clip.length);
    }
}
