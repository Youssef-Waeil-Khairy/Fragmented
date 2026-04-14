using UnityEngine;

public static class DisplaySettings
{
    public static void Apply(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        Screen.fullScreenMode = fullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;
    }
}
