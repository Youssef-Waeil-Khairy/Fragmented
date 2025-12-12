using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class SceneSwitcher : MonoBehaviour
    {
        [UsedImplicitly]
        public enum SceneIndex
        {
            MainMenu = 0,
            Tutorial = 1,
            LevelOne = 2,
        }
        [SerializeField] private SceneIndex _levelDestination;
        [SerializeField] private List<string> _whitelistTags =  new List<string>() {"Player"};
        private void OnTriggerEnter(Collider other)
        {
            if (_whitelistTags.Contains(other.tag))
            {
                SceneManager.LoadScene((int)_levelDestination);
            }
        }

        public void SwitchScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}