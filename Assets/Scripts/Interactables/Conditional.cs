using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class Conditional : MonoBehaviour, IRecordable
    {
        public UnityEvent OnConditionMet;
        public UnityEvent OnConditionNotMet;
        public UnityEvent OnConditionLocked;
        public UnityEvent OnConditionUnlocked;
        [Expandable] public Condition condition;

        [Foldout("Snapshot")][SerializeField] private bool snapshotIsLocked;
        [Foldout("Snapshot")][SerializeField] private bool snapshotBool;
        [Foldout("Snapshot")][SerializeField] private int snapshotInt;
        [Foldout("Snapshot")][SerializeField] private float snapshotFloat;
        [Foldout("Snapshot")][SerializeField] private List<bool> snapshotBoolSequence;
        [Foldout("Snapshot")][SerializeField] private List<int> snapshotIntSequence;
        [Foldout("Snapshot")][SerializeField] private List<float> snapshotFloatSequence;

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

        void OnLocked()
        {
            OnConditionLocked?.Invoke();
        }
        void OnUnlocked()
        {
            OnConditionUnlocked?.Invoke();
        }

        private void OnEnable()
        {
            if (condition != null)
            {
                condition.ValueUpdated += ConditionValueChanged;
                condition.ConditionLocked += OnLocked;
                condition.ConditionUnlocked += OnUnlocked;
            }
        }

        private void OnDisable()
        {
            if (condition != null)
            {
                condition.ResetToDefault();

                condition.ValueUpdated -= ConditionValueChanged;
                condition.ConditionLocked -= OnLocked;
                condition.ConditionUnlocked -= OnUnlocked;
            }
        }
        public void TakeSnapshot()
        {
            if (condition == null) return;

            snapshotIsLocked = condition.IsLocked;

            switch (condition.conditionType)
            {
                case Condition.ConditionType.Boolean:
                    snapshotBool = condition.BooleanValue;
                    break;
                case Condition.ConditionType.Float:
                    snapshotFloat = condition.FloatValue;
                    break;
                case Condition.ConditionType.Integer:
                    snapshotInt = condition.IntegerValue;
                    break;
                case Condition.ConditionType.SequenceBoolean:
                    snapshotBoolSequence = condition.ValueSequenceBoolean;
                    break;
                case Condition.ConditionType.SequenceFloat:
                    snapshotFloatSequence = condition.ValueSequenceFloat;
                    break;
                case Condition.ConditionType.SequenceInteger:
                    snapshotIntSequence = condition.ValueSequenceInteger;
                    break;
            }
        }
        public void TakeCheckpointSnapshot()
        {
            TakeSnapshot();
        }
        public void LoadSnapshot()
        {
            if (condition == null) return;

            condition.IsLocked = snapshotIsLocked;

            switch (condition.conditionType)
            {
                case Condition.ConditionType.Boolean:
                    condition.SetValue(snapshotBool);
                    break;
                case Condition.ConditionType.Float:
                    condition.SetValue(snapshotFloat);
                    break;
                case Condition.ConditionType.Integer:
                    condition.SetValue(snapshotInt);
                    break;
                case Condition.ConditionType.SequenceBoolean:
                    for (int bi = 0; bi < condition.ValueSequenceBoolean.Count; bi++)
                    {
                        condition.SetValue(bi, snapshotBoolSequence[bi]);
                    }
                    break;
                case Condition.ConditionType.SequenceFloat:
                    for (int fi = 0; fi < condition.ValueSequenceFloat.Count; fi++)
                    {
                        condition.SetValue(fi, snapshotFloatSequence[fi]);
                    }
                    break;
                case Condition.ConditionType.SequenceInteger:
                    for (int ii = 0; ii < condition.ValueSequenceInteger.Count; ii++)
                    {
                        condition.SetValue(ii, snapshotIntSequence[ii]);
                    }
                    break;
            }
        }
        public void LoadCheckpointSnapshot()
        {
            LoadSnapshot();
        }
    }
}