using System.Collections.Generic;
using UnityEngine;


public class MassLightFlickerActions : MonoBehaviour
{
    public List<LightPulse> Pulses;

    public void TogglePulsing(bool enable)
    {
        foreach (LightPulse pulse in Pulses)
        {
            if (enable) pulse.EnablePulsing();
            else pulse.DisablePulsing();
        }
    }

    public void TogglePulsingEnabled(bool enable)
    {
        foreach (LightPulse pulse in Pulses)
        {
            pulse.CanBeToggled = enable;
        }
    }
}