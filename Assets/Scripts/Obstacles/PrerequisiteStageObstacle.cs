using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Obstacles
{
    public class PrerequisiteStageObstacle : StagedObstacle
    {
        [Foldout("Prerequisites")] public List<bool> Prerequisites;
        public bool HasMetPrerequisite = false;
        public UnityEvent OnPrerequisitesMet, OnPrerequisitesUnMet;

        public override void GoToNextStage()
        {
            if (Prerequisites.Contains(false)) return;

            base.GoToNextStage();
        }

        public override void GoToPreviousStage()
        {
            if (Prerequisites.Contains(false)) return;

            base.GoToPreviousStage();
        }

        public override void GoToStage(int stage)
        {
            if (Prerequisites.Contains(false)) return;

            base.GoToStage(stage);
        }

        [UsedImplicitly]
        public void ActivatePrerequisite(int index)
        {
            Prerequisites[index] = true;

            if (Prerequisites.Contains(false))  return;

            if (!HasMetPrerequisite) OnPrerequisitesMet?.Invoke();
            HasMetPrerequisite = true;
        }

        [UsedImplicitly]
        public void DeactivatePrerequisite(int index)
        {
            Prerequisites[index] = false;

            if (HasMetPrerequisite)
            {
                OnPrerequisitesUnMet?.Invoke();
                HasMetPrerequisite = false;
            }
        }


    }
}