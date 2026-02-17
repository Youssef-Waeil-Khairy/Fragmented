using System.Collections.Generic;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractorPressurePlate : MonoBehaviour
    {
        [SerializeField] private bool _isInteracting = false;
        [SerializeField] private List<string> _whitelistTags = new List<string>() {"Player"};
        [SerializeField] private List<string> _blacklistTags = new List<string>() {};
        public UnityEvent OnBeginInteraction, OnEndInteraction, OnUnInteractOn, OnUnInteractOff;
        [ShowNonSerializedField] private bool _wasInteracted = false;
        [SerializeField] private int _interactionCountOn = 0;
        [SerializeField] private int _interactionCountOff = 0;

        void Start()
        {
            PlayerController.Instance.EchoMina.StartRecording += OnStartSignal;
            PlayerController.Instance.EchoMina.StopRecording += OnReverseSignal;
        }

        void OnDisable()
        {
            PlayerController.Instance.EchoMina.StartRecording -= OnStartSignal;
            PlayerController.Instance.EchoMina.StopRecording -= OnReverseSignal;
        }

        void OnReverseSignal()
        {
            if (_wasInteracted)
            {
                for (int i = 0; i < _interactionCountOn; i++)
                {
                    OnUnInteractOn?.Invoke();
                }

                for (int j = 0; j < _interactionCountOff; j++)
                {
                    OnUnInteractOff?.Invoke();
                }
                
                _isInteracting = false;
            }
        }

        void OnStartSignal()
        {
            _wasInteracted = false;
            if (_isInteracting)
            {
                Debug.LogError("Pressure plate started with interacting flag high");
            }
            _interactionCountOn = 0;
            _interactionCountOff = 0;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_whitelistTags.Contains(other.tag) && !_isInteracting)
            {
                Debug.Log(other.gameObject.name + " has entered pressure plate");
                OnBeginInteraction?.Invoke();
                _isInteracting = true;
                _wasInteracted = true;
                _interactionCountOn++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isInteracting)
            {
                Debug.Log(other.gameObject.name + " has exited pressure plate while it is interacted");
                OnEndInteraction?.Invoke();
                _isInteracting = false;
                _interactionCountOff++;
            }
        }
    }
}