using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InterractibleTrigerArea : MonoBehaviour
    {
        public UnityEvent OnTrigger;
        [Tag] public List<string> WhitelistTags;

        private void OnTriggerEnter(Collider other)
        {
            if (WhitelistTags.Contains(other.tag))
            {
                OnTrigger?.Invoke();
            }
        }

        [Button][UsedImplicitly]
        public void Activate()
        {
            OnTrigger?.Invoke();
        }
    }
}