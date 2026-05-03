using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    void Awake()
    {
        // Find all EventSystems in the scene
        EventSystem[] allEventSystems = FindObjectsOfType<EventSystem>();

        if (allEventSystems.Length > 1)
        {
            // Destroy all duplicates, keep only this one
            foreach (EventSystem es in allEventSystems)
            {
                if (es.gameObject != this.gameObject)
                {
                    Debug.Log("Duplicate EventSystem destroyed: " + es.gameObject.scene.name);
                    Destroy(es.gameObject);
                }
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}