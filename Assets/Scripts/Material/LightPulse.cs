using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public float minIntesnity = 0.5f;
    public float maxIntensity = 4f;
    public float pulseSpeed = 1f;
    private Light pointLight;
    void Start()
    {
        pointLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        pointLight.intensity = Mathf.Lerp(minIntesnity, maxIntensity,(Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
    }
}
