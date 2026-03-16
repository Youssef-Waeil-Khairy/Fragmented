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
                #if DEBUG
                Debug.LogError($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped on by {other.gameObject.name}");
                #endif

                _interactingObject = other.gameObject;

                OnBeginInteraction?.Invoke();
                _isInteracting = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isInteracting && other.gameObject == _interactingObject)
            {
                #if DEBUG
                Debug.LogError($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped off by {other.gameObject.name}");
                #endif

                _interactingObject =  null;
                OnEndInteraction?.Invoke();
                _isInteracting = false;
            }
        }

        private void RecordingStopped()
        {
            if (_isInteracting)
            {
                if (_interactingObject.CompareTag("Player"))
                {
                    Debug.LogError("Pressure plate is being interacted with by the player when recording stopped");
                    return;
                }
                else
                {
                    Debug.LogError("Pressure plate is being interacted with by the Echo Mins when recording stopped");
                    _isInteracting = false;
                    _interactingObject = null;
                    // BUG: [Pressure Plate] might need to call OnEndInteraction here as well
                }
            }
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