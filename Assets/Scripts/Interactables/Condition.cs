using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Conditional/New Condition", order = 0)]
    public class Condition : ScriptableObject
    {
        public enum ConditionType
        {
            Boolean,
            Integer,
            Float,
            SequenceBoolean,
            SequenceInteger,
            SequenceFloat,
        }

        public delegate void OnConditionMet();
        public event OnConditionMet OnConditionIsMet;

        public delegate void OnValueUpdated();
        public event OnValueUpdated ValueUpdated;
        
        public ConditionType conditionType;
        public bool IsMet = false;

        #region Values
        [ShowIf("ShowBool")] public bool BooleanCondition;
        [ShowIf("ShowBool")][SerializeField] private bool booleanValue;
        public bool BooleanValue
        {
            get {return booleanValue;}
            set
            {
                booleanValue = value;
                if (BooleanCondition == booleanValue)
                {
                    IsMet = true;
                    OnConditionIsMet?.Invoke();
                }
                ValueUpdated?.Invoke();
            }
        }
        
        [ShowIf("ShowInteger")] public int IntegerCondition;
        [ShowIf("ShowInteger")][SerializeField] private int integerValue;
        public int IntegerValue
        {
            get {return integerValue;}
            set
            {
                integerValue = value;
                if (IntegerCondition == integerValue)
                {
                    IsMet = true;
                    OnConditionIsMet?.Invoke();
                }
                ValueUpdated?.Invoke();
            }
        }
        
        [ShowIf("ShowFloat")] public float FloatCondition;
        [ShowIf("ShowFloat")][SerializeField] private float floatValue;
        public float FloatValue
        {
            get {return floatValue;}
            set
            {
                floatValue = value;
                if (Mathf.Approximately(FloatCondition, floatValue))
                {
                    IsMet = true;
                    OnConditionIsMet?.Invoke();
                }
                ValueUpdated?.Invoke();
            }
        }
        
        [ShowIf("ShowSequenceBoolean")] public List<bool> ConditionSequenceBoolean;
        [ShowIf("ShowSequenceBoolean")][SerializeField] private List<bool> valueSequenceBoolean;
        
        [ShowIf("ShowSequenceInteger")] public List<int> ConditionSequenceInteger;
        [ShowIf("ShowSequenceInteger")][SerializeField] private List<int> valueSequenceInteger;
        
        [ShowIf("ShowSequenceFloat")] public List<float> ConditionSequenceFloat;
        [ShowIf("ShowSequenceFloat")][SerializeField] private List<float> valueSequenceFloat;

        private bool ShowBool()
        {
            return conditionType == ConditionType.Boolean;
        }
        private bool ShowInteger()
        {
            return conditionType == ConditionType.Integer;
        }
        private bool ShowFloat()
        {
            return conditionType == ConditionType.Float;
        }
        private bool ShowSequenceBoolean()
        {
            return conditionType == ConditionType.SequenceBoolean;
        }
        private bool ShowSequenceInteger()
        {
            return conditionType == ConditionType.SequenceInteger;
        }
        private bool ShowSequenceFloat()
        {
            return conditionType == ConditionType.SequenceFloat;
        }
  #endregion

        #region Value Setting

        public void SetValue(bool value)
        {
            BooleanValue = value;
        }

        public void SetValue(int value)
        {
            IntegerValue = value;
        }

        public void SetValue(float value)
        {
            FloatValue = value;
        }

        public void SetValue(int index, bool value)
        {
            if (index < ConditionSequenceBoolean.Count)
            {
                valueSequenceBoolean[index] = value;

                if (SequencesAreCorrect(valueSequenceBoolean))
                {
                    IsMet = true;
                    OnConditionIsMet?.Invoke();
                }
                else
                {
                    IsMet = false;
                }
                ValueUpdated?.Invoke();
            }
            else
            {
                Debug.LogError("Tried set a condition sequence value outside the sequence range");
            }
        }
        
        public void SetValue(int index, int value)
        {
            if (index < ConditionSequenceInteger.Count)
            {
                valueSequenceInteger[index] = value;

                if (SequencesAreCorrect(valueSequenceInteger))
                {
                    IsMet = true;
                    OnConditionIsMet?.Invoke();
                }
                else
                {
                    IsMet = false;
                }
                ValueUpdated?.Invoke();
            }
            else
            {
                Debug.LogError("Tried set a condition sequence value outside the sequence range");
            }
        }
        
        public void SetValue(int index, float value)
        {
            if (index < ConditionSequenceFloat.Count)
            {
                valueSequenceFloat[index] = value;

                if (SequencesAreCorrect(valueSequenceFloat))
                {
                    IsMet = true;
                    OnConditionIsMet?.Invoke();
                }
                else
                {
                    IsMet = false;
                }
                ValueUpdated?.Invoke();
            }
            else
            {
                Debug.LogError("Tried set a condition sequence value outside the sequence range");
            }
        }

        private bool SequencesAreCorrect(List<bool> currentSequence)
        {
            bool result = true;
            for (int i = 0; i < ConditionSequenceBoolean.Count; i++)
            {
                if (ConditionSequenceBoolean[i] != currentSequence[i])
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        
        private bool SequencesAreCorrect(List<int> currentSequence)
        {
            bool result = true;
            for (int i = 0; i < ConditionSequenceInteger.Count; i++)
            {
                if (ConditionSequenceInteger[i] != currentSequence[i])
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        
        private bool SequencesAreCorrect(List<float> currentSequence)
        {
            bool result = true;
            for (int i = 0; i < ConditionSequenceBoolean.Count; i++)
            {
                if (!Mathf.Approximately(ConditionSequenceFloat[i], currentSequence[i]))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
  #endregion
    }
}
