using System;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractableLever : MonoBehaviour,  IInteractable
    {
        
        [SerializeField] private UnityEvent onInteract;
        [SerializeField] private UnityEvent onUnInteract;
        [SerializeField] private UnityEvent onStartPreview;
        [SerializeField] private UnityEvent onStopPreview;
        [BoxGroup("Recording")][SerializeField] private bool _wasInteracted = false;
        [BoxGroup("Recording")][SerializeField] private int _interactionCount = 0;

        private void Start()
        {
            PlayerController.Instance.EchoMina.StartRecording += EchoMinaStartRecording;
            PlayerController.Instance.EchoMina.StopRecording  += EchoMinaOnStopRecording;
        }

        private void OnDisable()
        {
            PlayerController.Instance.EchoMina.StartRecording -= EchoMinaStartRecording;
            PlayerController.Instance.EchoMina.StopRecording  -= EchoMinaOnStopRecording;
        }
        void EchoMinaOnStopRecording()
        {
            if (_wasInteracted)
            {
                for (int i = 0; i < _interactionCount; i++)
                {
                    UnInteract(PlayerController.Instance.EchoMina.EchoInteractor);
                }
            }
        }

        void EchoMinaStartRecording()
        {
            _wasInteracted = false;
            _interactionCount = 0;
        }

        public bool CanInteract()
        {
            return true;
        }
        public void Interact(Interactor interactor)
        {
            onInteract?.Invoke();
            _wasInteracted = true;
            _interactionCount++;
        }

        public void UnInteract(Interactor interactor)
        {
            onUnInteract?.Invoke();
        }

        public void StartPreview()
        {
            onStartPreview?.Invoke();
        }
        public void StopPreview()
        {
            onStopPreview?.Invoke();
        }
    }
}