using System;
using EchoMina.Original;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractableLever : MonoBehaviour, IInteractable
    {

        [SerializeField] private UnityEvent onInteract;
        [SerializeField] private UnityEvent onUnInteract;
        [SerializeField] private UnityEvent onStartPreview;
        [SerializeField] private UnityEvent onStopPreview;
        [BoxGroup("Recording")][SerializeField] private bool _wasInteracted = false;
        [BoxGroup("Recording")][SerializeField] private int _interactionCount = 0;

    #region Unity Functions

        private void OnDisable()
        {
            OriginalEchoMina.Instance.StartRecording -= EchoMinaStartRecording;
            OriginalEchoMina.Instance.StopRecording -= EchoMinaOnStopRecording;
        }

    #endregion

    #region Event Methods

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

    #endregion

    #region IInteractable Methods

        public void BindObject()
        {
            OriginalEchoMina.Instance.StartRecording += EchoMinaStartRecording;
            OriginalEchoMina.Instance.StopRecording += EchoMinaOnStopRecording;
        }
        public bool CanInteract(Interactor interactor)
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
        public bool IsLockable()
        {
            return false;
        }
        public bool IsLocked()
        {
            return false;
        }
        public void Lock(Interactor interactor)
        {
            return;
        }
        public void Unlock(Interactor interactor)
        {
            return;
        }

    #endregion
    }
}