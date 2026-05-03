using NaughtyAttributes;
using UnityEngine;

namespace Interactables
{
    public class ConditionalSequencerSetter : MonoBehaviour
    {
        [Expandable][SerializeField] private Condition condition;
        [SerializeField] private int index;
        [SerializeField][ReadOnly] private bool val;

        public void ToggleBooleanSequence()
        {
            if (condition)
            {
                val = !val;
                condition.SetValue(index, val);
            }
        }
        
        public void UpdateBooleanSequence(bool value)
        {
            if (condition)
            {
                condition.SetValue(index, value);
            }
        }

        public void UpdateIntegerSequence(int value)
        {
            if (condition)
            {
                condition.SetValue(index, value);
            }
        }

        public void UpdateFloatSequence(float value)
        {
            if (condition)
            {
                condition.SetValue(index, value);
            }
        }
    }
}