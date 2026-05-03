using UnityEngine;

public class SkyboxAndObjectToggle : MonoBehaviour
{
    [Header("Skybox Settings")]
    public Material skyboxOnEnter;
    public Material skyboxOnExit;

    [Header("On Trigger ENTER")]
    [Tooltip("These objects will be turned ON when you enter, and OFF when you leave.")]
    public GameObject[] enableOnEnter;

    [Tooltip("These objects will be turned OFF when you enter, and ON when you leave.")]
    public GameObject[] disableOnEnter;

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger for the Player
        if (!other.CompareTag("Player")) return;

        // 1. Change Skybox
        if (skyboxOnEnter != null)
        {
            RenderSettings.skybox = skyboxOnEnter;
            DynamicGI.UpdateEnvironment();
        }

        // 2. Enable the 'Enable' group
        foreach (GameObject obj in enableOnEnter)
        {
            if (obj != null) obj.SetActive(true);
        }

        // 3. Disable the 'Disable' group
        foreach (GameObject obj in disableOnEnter)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Only trigger for the Player
        if (!other.CompareTag("Player")) return;

        // 1. Revert Skybox
        if (skyboxOnExit != null)
        {
            RenderSettings.skybox = skyboxOnExit;
            DynamicGI.UpdateEnvironment();
        }

        // 2. Reverse: Turn OFF the 'Enable' group
        foreach (GameObject obj in enableOnEnter)
        {
            if (obj != null) obj.SetActive(false);
        }

        // 3. Reverse: Turn ON the 'Disable' group
        foreach (GameObject obj in disableOnEnter)
        {
            if (obj != null) obj.SetActive(true);
        }
    }
}
