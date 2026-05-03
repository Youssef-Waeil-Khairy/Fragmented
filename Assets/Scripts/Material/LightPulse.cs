using NaughtyAttributes;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public float minIntesnity = 0.5f;
    public float maxIntensity = 4f;
    public float pulseSpeed = 1f;
    private Light pointLight;

    // Jan Willem Changes Start
    public bool IsPulsing = true;
    public bool CanBeToggled = false;
    // Jan Willem Changes End
    void Start()
    {
        pointLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPulsing)
        {
            pointLight.intensity = Mathf.Lerp(minIntesnity, maxIntensity,(Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        }
    }

    public void TogglePulsingEnabled(bool value)
    {
        CanBeToggled = value;
    }

    [Button]
    public void EnablePulsing()
    {
        if (!CanBeToggled)
        {
            return;
        }
        IsPulsing = true;
    }

    [Button]
    public void DisablePulsing()
    {
        if (!CanBeToggled)
        {
            return;
        }

        IsPulsing = false;
        pointLight.intensity = maxIntensity;
    }
}