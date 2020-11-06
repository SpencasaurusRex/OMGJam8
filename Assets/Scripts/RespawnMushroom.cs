using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnMushroom : MonoBehaviour
{
    public GameObject Mask;

    public float WaitTime = 0.5f;
    float timeWaited;
    
    public void Respawn()
    {
        timeWaited = 0;
        Mask.SetActive(true);
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
