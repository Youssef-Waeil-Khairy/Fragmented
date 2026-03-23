using System;
using EchoMina.Original;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Interactables
{
    public class InteractableLever : MonoBehaviour, IInteractable, IBindable
    {

        [SerializeField] private bool isInteracted;
        [SerializeField] private GameObject previewPanel;
        [SerializeField] private UnityEvent onInteract;
        [SerializeField] private UnityEvent onUnInteract;
        [SerializeField] private UnityEvent onStartPreview;
        [SerializeField] private UnityEvent onStopPreview;


    #region IInteractable Methods

        private void StopRecording() {isInteracted = false;}
        public void BindObject()
        {
            OriginalEchoMina.Instance.StopRecording += StopRecording;
        }
        public void UnBindObject()
        {
            OriginalEchoMina.Instance.StopRecording -= StopRecording;
        }

        public bool CanInteract(Interactor interactor)
        {
            return this.enabled;
        }
        public void Interact(Interactor interactor)
        {
            if (isInteracted)
            {
                onUnInteract.Invoke();
                isInteracted = false;
            }
            else if (!isInteracted)
            {
                onInteract?.Invoke();
                isInteracted = true;
            }
        }

        public void StartPreview()
        {
            previewPanel.SetActive(true);
            onStartPreview?.Invoke();
        }
        public void StopPreview()
        {
            previewPanel.SetActive(false);
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

        [Button]
        public void DebugInteract()
        {
            if (isInteracted)
            {
                onUnInteract.Invoke();
                isInteracted = false;
            }
            else if (!isInteracted)
            {
                onInteract?.Invoke();
                isInteracted = true;
            }
        }
    }
}