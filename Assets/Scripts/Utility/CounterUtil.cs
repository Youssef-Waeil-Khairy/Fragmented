using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility
{
    /// <summary>
    /// A Utility script for the sole purpose of displaying a number that can be increased and decreased
    /// </summary>
    public class CounterUtil : MonoBehaviour
    {
        [ResizableTextArea] public string Notes;
        public int counter = 0;
        public int incrementBy;
        public int decrementBy;

        [Button] public void Increment() { counter += incrementBy; }
        [Button] public void Decrement() { counter -= decrementBy; }
    }
}
