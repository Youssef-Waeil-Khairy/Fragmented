using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class Conditional : MonoBehaviour
    {
        public UnityEvent OnConditionMet;
        public UnityEvent OnConditionNotMet;
        [Expandable][SerializeField] private Condition condition;

        void ConditionValueChanged()
        {
            if (condition.IsMet)
            {
                OnConditionMet?.Invoke();
            }
            else
            {
                OnConditionNotMet?.Invoke();
            }
        }
        
        private void OnEnable()
        {
            if (condition != null)
            {
                condition.ValueUpdated += ConditionValueChanged;
            }
        }

        private void OnDisable()
        {
            if (condition != null)
            {
                condition.ValueUpdated += ConditionValueChanged;
            }
        }
    }
}
