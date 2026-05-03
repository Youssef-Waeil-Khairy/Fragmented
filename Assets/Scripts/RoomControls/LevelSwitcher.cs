using System;
using JetBrains.Annotations;
using MainMenu;
using UnityEngine;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine.SceneManagement;
using Utility;

namespace RoomControls
{
    public class LevelSwitcher : MonoBehaviour
    {
        [Scene][SerializeField] private int level;
        [Tag][SerializeField] private string allowedTag;

        [SerializeField] private bool isGoingToScene = false;
        public bool IsGoingToSettingsScene = false;

        private void OnEnable()
        {
            if (ScreenTransitioner.Instance != null)
            {
                StartupLogger.LogEnable($"{name} Subscribing level transition events", "LevelSwitcher");
                ScreenTransitioner.Instance.EndFadeIn += DoGoToScene;
            }
        }
        private void OnDisable()
        {
            if (ScreenTransitioner.Instance != null)
            {
                StartupLogger.LogDisable($"{name} Unsubscribing level transition events", "LevelSwitcher");

                ScreenTransitioner.Instance.EndFadeIn -= DoGoToScene;
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
            if (!isGoingToScene)
            {
                return;
            }

            isGoingToScene = false; // Reset this to false so that if we are reused (mostly for persistent switchers) we don't continue thinking we are being used

            if (IsGoingToSettingsScene)
            {
                PauseMenu.Instance.TakeSnapshot();
            }

            SceneManager.LoadScene(level);
        }

        [Button][UsedImplicitly]
        public void GoToScene()
        {
            Debug.Log($"Go to scene: {level} from {SceneManager.GetActiveScene().buildIndex}");
            //PauseMenu.Instance.CurrentScene = level;
            isGoingToScene = true;
            ScreenTransitioner.Instance.DoFadeIn();
        }

        public void GoToSettingsScene()
        {
            Debug.Log("level switcher going to settings scene");
            isGoingToScene = true;
            PauseMenu.Instance.CurrentScene = SceneManager.GetActiveScene().buildIndex;
            PauseMenu.Instance.TakeSnapshot();

            GoToScene();
        }

        public void LeaveSettingsScene()
        {
            Debug.Log($"Leave settings scene");
            isGoingToScene = true;
            PauseMenu.Instance.ShouldLoadSnapShot = true;
            level = PauseMenu.Instance.CurrentScene;

            GoToScene();
        }

        public void CloseGame()
        {
            Application.Quit();
        }
    }
}