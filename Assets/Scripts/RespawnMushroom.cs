using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnMushroom : MonoBehaviour
{
    public GameObject Mask;
    public float WaitTime = 1f;
    
    AudioSource source;
    float timeWaited;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Respawn()
    {
        timeWaited = 0;
        Mask.SetActive(true);
    }

    public void Pop()
    {
        source.PlayOneShot(source.clip);
    }

    void Update()
    {
        timeWaited += Time.deltaTime;
        if (timeWaited >= WaitTime)
        {
            Mask.SetActive(false);
        }
    }
}
