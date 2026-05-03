using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuLoader : MonoBehaviour
{
    void Awake()
    {
        // Load PauseMenu scene additively so it persists everywhere
        if (!SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        }

        Debug.Log($"Active scene: {SceneManager.GetActiveScene().name}");
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
    }
}