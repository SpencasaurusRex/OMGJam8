using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public enum FluctuateType
{
    Sine
}

public class LightIntensityFluctuate : MonoBehaviour
{
    public Light2D Light;
    public FluctuateType Type;
    

    [Header("Sine")]
    public float Frequency;
    public float MinIntensity;
    public float MaxIntensity;
    
    float time;

    void Start()
    {
        if (Light == null)
        {
            Light = GetComponent<Light2D>();
        }
    }

    void Update()
    {
        time += Time.deltaTime;
        
        if (Type == FluctuateType.Sine)
        {
            float t = Mathf.InverseLerp(-1, 1, Mathf.Sin(Frequency * Mathf.PI * 2 * time));
            Light.intensity = Mathf.Lerp(MinIntensity, MaxIntensity, t);
        }
    }
}
