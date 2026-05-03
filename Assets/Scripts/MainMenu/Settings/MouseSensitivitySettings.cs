using UnityEngine;

public class MouseSensitivitySettings : MonoBehaviour
{
    //med = 1 , large = 2

    public static float Medium = 1f;
    public static float Large = 2f;

    //this is called whenever u read mouse input like transform or roate

    public static float GetSensitivity()
    {
        return GameSettingsManager.Instance != null
            ? GameSettingsManager.Instance.MouseSensitivity
            : Medium;
    }
}
