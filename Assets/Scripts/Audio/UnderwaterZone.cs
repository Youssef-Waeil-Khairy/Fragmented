using UnityEngine;
using UnityEngine.Audio;

public class UnderwaterZone : MonoBehaviour
{
    public AudioMixer audioMixer;

    public float normalCutoff = 22000f;
    public float underwaterCutoff = 800f;
    public float transitionSpeed = 3f;

    private float targetCutoff;
    private float currentCutoff;

    // All 4 mixer parameter names
    private string[] mixerParams = new string[]
    {
        "MusicLowPass",
        "SFXLowPass",
        "VoiceLowPass",
        "NotifLowPass"
    };

    void Start()
    {
        currentCutoff = normalCutoff;
        targetCutoff = normalCutoff;
    }

    void Update()
    {
        currentCutoff = Mathf.Lerp(currentCutoff, targetCutoff, Time.deltaTime * transitionSpeed);

        // Apply to all groups
        foreach (string param in mixerParams)
        {
            audioMixer.SetFloat(param, currentCutoff);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("EchoMina"))
            targetCutoff = underwaterCutoff;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("EchoMina"))
            targetCutoff = normalCutoff;
    }
}