using System;
using JetBrains.Annotations;
using MainMenu;
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
        public ScreenTransitioner Transitioner;
        public bool IsInterLevelTransition;

        public bool IsLeavingSettingsScene = false;
        public bool IsGoingToSettingsScene = false;

        private void OnEnable()
        {
            if (Transitioner != null)
            {
                Transitioner.EndFadeIn += DoGoToScene;
            }
        }
        private void OnDisable()
        {
            if (Transitioner != null)
            {
                Transitioner.EndFadeIn -= DoGoToScene;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(allowedTag))
            {
                GoToScene();
            }
        }

        private void DoGoToScene()
        {
            if (IsLeavingSettingsScene)
            {
                SceneManager.LoadScene(PauseMenu.Instance.CurrentScene);
            }
            else
            {
                SceneManager.LoadScene(level);
            }
        }

        [Button][UsedImplicitly]
        public void GoToScene()
        {
            Debug.Log($"Loading scene: {level} from {SceneManager.GetActiveScene().buildIndex}");
            PauseMenu.Instance.CurrentScene = level;

            if (Transitioner != null)
            {
                Transitioner.DoFadeIn();
            }
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

            if (Transitioner != null)
            {
                Transitioner.DoFadeIn();
                PauseMenu.Instance.ShouldLoadSnapShot = true;
            }
        }

        public void CloseGame()
        {
            Application.Quit();
        }
    }
}