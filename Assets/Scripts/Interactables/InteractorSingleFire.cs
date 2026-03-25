using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractorSingleFire : MonoBehaviour,  IInteractable
    {
        public UnityEvent OnInteract, OnPreviewStart,  OnPreviewEnd;
        public Outline PreventOutline;
        public GameObject PreviewPanel;

        private void OnEnable()
        {
            PreventOutline = GetComponent<Outline>();
            PreventOutline.enabled = false;
        }

        public bool CanInteract(Interactor interactor)
        {
            return true;
        }
        public void Interact(Interactor interactor)
        {
            OnInteract?.Invoke();
        }
        public void StartPreview()
        {
            PreventOutline.enabled = true;
            PreviewPanel.SetActive(true);
            OnPreviewStart?.Invoke();

        }
        public void StopPreview()
        {
            PreventOutline.enabled = false;
            PreviewPanel.SetActive(false);
            OnPreviewEnd?.Invoke();
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
            Debug.Log("You should not be seeing this. The Single Fire interactible is not lockable and you tried locking it.");
        }
        public void Unlock(Interactor interactor)
        {
            Debug.Log("You should not be seeing this. The Single Fire interactible is not lockable and you tried unlocking it.");
        }
    }
}