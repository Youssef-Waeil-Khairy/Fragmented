using JetBrains.Annotations;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

namespace RoomControls
{
    public class LevelSwitcher : MonoBehaviour
    {
        [Scene][SerializeField] private int level;
        [Tag][SerializeField] private string allowedTag;

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
            SceneManager.LoadScene(level);
        }

        public void CloseGame()
        {
            Application.Quit();
        }
    }
}