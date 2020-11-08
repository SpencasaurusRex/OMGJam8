using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    void Start()
    {
        GameController.instance.StartFadeIn(GameController.FadeContext.LevelChange);
        Destroy(gameObject);
    }
}
