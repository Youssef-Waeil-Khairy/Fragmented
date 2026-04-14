using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Dispaly")]
    public Button fullscreenButton;
    public Button windowedBUtton;

    [Header("Mouse Sensitivity")]
    public Slider mouseSensitivtySlider;

    [Header("Sound Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;
    public Slider notifSlider;

    void Start()
    {


        var s = GameSettingsManager.Instance;

       
        // Sound

        musicSlider.value = s.MusicVolume;
        sfxSlider.value = s.SFXVolume;
        voiceSlider.value = s.VoiceVolume;
        notifSlider.value = s.NotificationVolume;

        //display
        fullscreenButton.onClick.AddListener(() =>
        {
            if (s.IsFullscreen) return;
            s.SetFullscreen(true);
        });

        windowedBUtton.onClick.AddListener(() =>
        {
            if (s.IsFullscreen) return;
            s.SetFullscreen(false);
        });

        mouseSensitivtySlider.onValueChanged.AddListener(s.SetMouseSensitivity);

        musicSlider.onValueChanged.AddListener(s.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(s.SetSFXVolume);
        voiceSlider.onValueChanged.AddListener(s.SetVoiceVolume);
        notifSlider.onValueChanged.AddListener(s.SetNotificationVolume);
    }
}
