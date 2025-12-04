using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    [RequireComponent(typeof(BoxCollider))]
    public class Interactor : MonoBehaviour
    {
        private IInteractable _interactable;
        [SerializeField] private Vector3 _interactionBox = Vector3.one;
        [SerializeField] private Vector3 _interactionOffset = Vector3.one;

        private void Start()
        {
            BoxCollider _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.size = _interactionBox;
            _collider.center = _interactionOffset;
        }

        public void OnInteract(InputValue context)
        {
            if (_interactable != null && _interactable.CanInteract())
            {
                _interactable.Interact(this);
            }
        }

        public void InteractCommand()
        {
            if (_interactable != null && _interactable.CanInteract())
            {
                _interactable.Interact(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log(other.gameObject.name + " has exited");
            var go = other.GetComponent<IInteractable>();
            if (go != null)
            {
                if (go == _interactable)
                {
                    go.StopPreview();
                    _interactable = null;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var go = other.GetComponent<IInteractable>();
            if (go != null)
            {
                if (_interactable == null)
                {
                    _interactable = go;
                    _interactable.StartPreview();
                    Debug.Log("Interacting with " + go);
                }
            }
        }



        private void OnDrawGizmos()
        {
            Gizmos.color = Color.darkGreen;
            Gizmos.DrawWireCube(transform.position + _interactionOffset, _interactionBox);
        }
    }
}