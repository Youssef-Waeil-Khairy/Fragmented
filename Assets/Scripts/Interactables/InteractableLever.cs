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
        [SerializeField] private bool isInteracted;
        

    #region IInteractable Methods

        public void BindObject()
        {
            
        }
        
        public bool CanInteract(Interactor interactor)
        {
            return true;
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
        public void UnInteract(Interactor interactor)
        {
            
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