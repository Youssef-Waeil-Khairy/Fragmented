using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class InteractableLever : MonoBehaviour,  IInteractable
    {
        
        [SerializeField] private UnityEvent onInteract;
        [SerializeField] private UnityEvent onStartPreview;
        [SerializeField] private UnityEvent onStopPreview;
        public bool CanInteract()
        {
            return true;
        }
        public void Interact(Interactor interactor)
        {
            onInteract?.Invoke();
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