using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility
{
    /// <summary>
    /// A Utility script for easily toggling a number of GameObjects between on and off
    /// </summary>
    public class TogglerUtil : MonoBehaviour
    {
        [ResizableTextArea] public string Notes;
        [FormerlySerializedAs("ToToggle")]
        public List<GameObject> toToggle;

        [Button]
        public void ToggleAll()
        {
            foreach (var item in toToggle)
            {
                item.SetActive(!item.activeSelf);
            }
        }

        public void ToggleOn()
        {
            foreach (GameObject item in toToggle)
            {
                item.SetActive(true);
            }
        }

        public void ToggleOff()
        {
            foreach (GameObject item in toToggle)
            {
                item.SetActive(false);
            }
        }
    }
}