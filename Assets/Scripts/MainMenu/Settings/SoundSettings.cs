using UnityEngine;
using UnityEngine.Audio;

public static class SoundSettings

{
    // draging audiomixer into the settings manger so it pass here

    public static AudioMixer Mixer;

    public static void Apply(float music, float sfx, float voice, float notif)
    {
        if (Mixer == null) return;

        Mixer.SetFloat("MusicVol", ToDecibels(music));
        Mixer.SetFloat("SFXVol", ToDecibels(sfx));
        Mixer.SetFloat("VoiceVol", ToDecibels(voice));
        Mixer.SetFloat("NotifVol", ToDecibels(notif));
    }

    static float ToDecibels(float linear)
    {
        return linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;
    }
}
