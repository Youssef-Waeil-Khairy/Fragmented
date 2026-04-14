using UnityEngine;

public class SkyboxAndObjectToggle : MonoBehaviour
{
    [Header("Skybox Settings")]
    public Material skyboxOnEnter;
    public Material skyboxOnExit;

    [Header("Objects To Toggle (Drag manually)")]
    public GameObject[] objectsToToggle;

    [Header("OR Use Tag Instead")]
    public bool useTagInstead = false;
    public string tagToToggle = "DisableGroup";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Skybox change
        if (skyboxOnEnter != null)
        {
            RenderSettings.skybox = skyboxOnEnter;
            DynamicGI.UpdateEnvironment();
        }

        // Disable objects
        if (useTagInstead)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tagToToggle);
            foreach (GameObject obj in objs)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Skybox revert
        if (skyboxOnExit != null)
        {
            RenderSettings.skybox = skyboxOnExit;
            DynamicGI.UpdateEnvironment();
        }

        // Re-enable objects
        if (useTagInstead)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tagToToggle);
            foreach (GameObject obj in objs)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }
}