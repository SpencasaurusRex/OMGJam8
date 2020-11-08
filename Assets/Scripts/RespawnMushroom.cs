using UnityEngine;

public class RespawnMushroom : MonoBehaviour
{
    public GameObject Mask;
    public float WaitTime = 1f;
    public OneShotSound PopSound;
    public OneShotSound MarkSound;
    
    public float Direction;
    float timeWaited;

    public void Respawn()
    {
        timeWaited = 0;
        Mask.SetActive(true);
    }

    public void Pop()
    {
        Instantiate(PopSound, gameObject.transform);
    }

    public void Mark()
    {
        print("Mark");
        Instantiate(MarkSound, transform);
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
