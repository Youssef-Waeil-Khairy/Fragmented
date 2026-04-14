using UnityEngine;
using UnityEngine.Audio;

public class GameSettingsManager : MonoBehaviour
{

    public static GameSettingsManager Instance;

    // display
    public bool IsFullscreen { get; private set; }

    // Mouse sensitivity
    public float MouseSensitivity { get; private set; }

    // Sound
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public float VoiceVolume { get; private set; }
    public float NotificationVolume { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    void Awake()
    {
        if (Instance != null && Instance != true) { Destroy(gameObject); return;  }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        SoundSettings.Mixer = audioMixer;

        LoadAll();
    }

    void LoadAll()
    {
        IsFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSens", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVol", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVol", 1f);
        VoiceVolume = PlayerPrefs.GetFloat("VoiceVol", 1f);
        NotificationVolume = PlayerPrefs.GetFloat("NotifVol", 1f);
        ApplyAll();

    }

    public void ApplyAll()
    {
        DisplaySettings.Apply(IsFullscreen);
        SoundSettings.Apply(MusicVolume, SFXVolume, VoiceVolume, NotificationVolume);
    }

    public void SetFullscreen(bool value)
    {
        IsFullscreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
        DisplaySettings.Apply(value);
    }

    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat("MouseSens", value);
    }

    public void SetMusicVolume(float value) 
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat("MusicVol", value);
        SoundSettings.Apply(MusicVolume, SFXVolume, VoiceVolume, NotificationVolume);
    }



    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
        PlayerPrefs.SetFloat("SFXVol", value);
        SoundSettings.Apply(MusicVolume, SFXVolume, VoiceVolume, NotificationVolume);
    }

    public void SetVoiceVolume(float value)
    {
        VoiceVolume = value;
        PlayerPrefs.SetFloat("VoiceVol", value);
        SoundSettings.Apply(MusicVolume, SFXVolume, VoiceVolume, NotificationVolume);
    }

    public void SetNotificationVolume(float value)
    {
        NotificationVolume = value;
        PlayerPrefs.SetFloat("NotifVol", value);
        SoundSettings.Apply(MusicVolume, SFXVolume, VoiceVolume, NotificationVolume);
    }
}
