using System.Collections.Generic;
using EchoMina.Original;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Utility;
// ReSharper disable InconsistentNaming

namespace Interactables
{
    public class InteractorPressurePlate : MonoBehaviour, IBindable
    {
        [SerializeField] private bool _isInteracting = false;
        [Tag][SerializeField] private List<string> _whitelistTags = new List<string>() {"Player"};
        [SerializeField] private GameObject _interactingObject;
        public UnityEvent OnBeginInteraction, OnEndInteraction;

        private void OnTriggerEnter(Collider other)
        {
            if (_whitelistTags.Contains(other.tag) && !_isInteracting)
            {
                Debug.Log($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped on by {other.gameObject.name}");
                _interactingObject = other.gameObject;
                OnBeginInteraction?.Invoke();
                _isInteracting = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isInteracting && other.gameObject == _interactingObject)
            {
                Debug.Log($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped off by {other.gameObject.name}");
                _interactingObject =  null;
                OnEndInteraction?.Invoke();
                _isInteracting = false;
            }
        }

        private void RecordingStopped()
        {
            _isInteracting = false;
            _interactingObject = null;
        }

        private void PlaybackStopped()
        {
            if (!_isInteracting) return;

            OnEndInteraction?.Invoke();
            _interactingObject = null;
            _isInteracting = false;
        }
        public void BindObject()
        {
            OriginalEchoMina.Instance.StopPlayback += PlaybackStopped;
            OriginalEchoMina.Instance.StopRecording += RecordingStopped;
        }
        public void UnBindObject()
        {
            OriginalEchoMina.Instance.StopPlayback -= PlaybackStopped;
            OriginalEchoMina.Instance.StopRecording -= RecordingStopped;
        }
    }
}