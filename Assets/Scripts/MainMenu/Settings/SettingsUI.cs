using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Dispaly")]
    public Toggle fullscreenToggle;
    public Toggle windowedToggle;

    [Header("Mouse Sensitivity")]
    public Toggle mediumToggle;
    public Toggle largeToggle;

    [Header("Sound Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;
    public Slider notifSlider;

    void Start()
    {
        var s = GameSettingsManager.Instance;

        //display

        fullscreenToggle.isOn = s.IsFullscreen;
        windowedToggle.isOn = !s.IsFullscreen;

        // Mouse

        mediumToggle.isOn = s.MouseSensitivity <= MouseSensitivitySettings.Medium;
        largeToggle.isOn = s.MouseSensitivity > MouseSensitivitySettings.Medium;

        // Sound

        musicSlider.value = s.MusicVolume;
        sfxSlider.value = s.SFXVolume;
        voiceSlider.value = s.VoiceVolume;
        notifSlider.value = s.NotificationVolume;

        // listners

        fullscreenToggle.onValueChanged.AddListener(v => { if (v) s.SetFullscreen(true); });
        windowedToggle.onValueChanged.AddListener(v => { if (v) s.SetFullscreen(false); });

        mediumToggle.onValueChanged.AddListener(v => { if (v) s.SetMouseSensitivity(MouseSensitivitySettings.Medium); });
        largeToggle.onValueChanged.AddListener(v => { if (v) s.SetMouseSensitivity(MouseSensitivitySettings.Large); });

        musicSlider.onValueChanged.AddListener(s.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(s.SetSFXVolume);
        voiceSlider.onValueChanged.AddListener(s.SetVoiceVolume);
        notifSlider.onValueChanged.AddListener(s.SetNotificationVolume);
    }
}
