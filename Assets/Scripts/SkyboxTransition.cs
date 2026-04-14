using UnityEngine;

public class SkyboxTransition : MonoBehaviour
{
    [Header("skybox Settings")]
    public Material skyboxOnEnter;
    public Material skyboxOnExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (skyboxOnEnter != null)
            {
                RenderSettings.skybox = skyboxOnEnter;

                DynamicGI.UpdateEnvironment();

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (skyboxOnEnter != null)
            {
                RenderSettings.skybox = skyboxOnExit;

                DynamicGI.UpdateEnvironment();

            }
        }
    }
}
