using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractorPressurePlate : MonoBehaviour
    {
        [SerializeField] private List<string> _whitelistTags = new List<string>() {"Player"};
        [SerializeField] private List<string> _blacklistTags = new List<string>() {};
        public UnityEvent OnBeginInteraction, OnEndInteraction;
        [SerializeField] private bool _isInteracting = false;
        private void OnTriggerEnter(Collider other)
        {
            if (_blacklistTags.Contains(other.tag))
            {
                return;
            }

            if (_whitelistTags.Contains(other.tag) && !_isInteracting)
            {
                OnBeginInteraction?.Invoke();
                _isInteracting = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isInteracting)
            {
                if (_whitelistTags.Contains(other.tag))
                {
                    OnEndInteraction?.Invoke();
                    _isInteracting = false;
                }
            }
        }
    }
}