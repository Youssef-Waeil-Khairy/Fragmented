using JetBrains.Annotations;
using UnityEngine;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine.SceneManagement;

namespace RoomControls
{
    public class LevelSwitcher : MonoBehaviour
    {
        [Scene][SerializeField] private int level;
        [Tag][SerializeField] private string allowedTag;
        public bool IsInterLevelTransition;

        public bool IsLeavingSettingsScene = false;
        public bool IsGoingToSettingsScene = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(allowedTag))
            {
                GoToScene();
            }
        }

        [Button][UsedImplicitly]
        public void GoToScene()
        {
            Debug.Log($"Loading scene: {level} from {SceneManager.GetActiveScene().buildIndex}");

            SceneManager.LoadScene(level);
            PauseMenu.Instance.CurrentScene = level;
        }

        public void GoToSettingsScene()
        {
            Debug.Log("level switcher going to settings scene");
            PauseMenu.Instance.CurrentScene = SceneManager.GetActiveScene().buildIndex;
            PauseMenu.Instance.TakeSnapshot();
            Debug.Log($"Loading scene: {level} from {SceneManager.GetActiveScene().buildIndex}");
            SceneManager.LoadScene(level);
        }

        public void LeaveSettingsScene()
        {
            Debug.Log($"Loading scene: {PauseMenu.Instance.CurrentScene} from {SceneManager.GetActiveScene().buildIndex}");
            SceneManager.LoadScene(PauseMenu.Instance.CurrentScene);

            while (!SceneManager.GetActiveScene().isLoaded)
            {
                Debug.Log("Waiting for scene to load");
            }
            PauseMenu.Instance.LoadSnapshot();
        }

        public void CloseGame()
        {
            Application.Quit();
        }
    }
}