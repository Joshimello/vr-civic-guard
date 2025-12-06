using UnityEngine;

public class PulseLight : MonoBehaviour
{
    public Light targetLight;
    public float speed = 2f;
    public float minIntensity = 1f;
    public float maxIntensity = 4f;

    void Update()
    {
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * speed) + 1) / 2);
    }
}
