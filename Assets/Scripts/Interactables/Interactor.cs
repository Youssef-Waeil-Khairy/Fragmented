using System;
using EchoMina.Original;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    [RequireComponent(typeof(BoxCollider))]
    public class Interactor : MonoBehaviour
    {
        [ShowNonSerializedField] private IInteractable _interactable;
        [SerializeField] private Vector3 _interactionBox = Vector3.one;
        [SerializeField] private Vector3 _interactionOffset = Vector3.one;

        public bool IsMina = false;
        [Foldout("Echo Mina")][SerializeField] private float _timeSinceLastInteraction = 0f;
        [Foldout("Echo Mina")][SerializeField] private OriginalEchoMina _originalEchoMina;

        private void Start()
        {
            BoxCollider _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.size = _interactionBox;
            _collider.center = _interactionOffset;
        }

        void Update()
        {
            if (_originalEchoMina.IsRecording && IsMina) _timeSinceLastInteraction += Time.deltaTime;
        }

        private void OnDisable()
        {
            if (_interactable != null)
            {
                _interactable?.StopPreview();
                _interactable = null;
            }
        }

        public void OnInteract(InputValue context)
        {
            Debug.Log("Interact" + gameObject.name);
            if (IsMina)
            {
                _originalEchoMina.SaveInteraction(_timeSinceLastInteraction);
                _timeSinceLastInteraction = 0f;
                _originalEchoMina.EchoInteractor.InteractCommand();
            }
            
            if (_interactable != null && _interactable.CanInteract())
            {
                _interactable.Interact(this);
            }
        }

        public void InteractCommand()
        {
            Debug.Log("InteractCommand");
            if (_interactable != null && _interactable.CanInteract())
            {
                _interactable.Interact(this);
            }
        }

        public void StartRecording()
        {
            _timeSinceLastInteraction = 0f;
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
                    Debug.Log("Entered interaction range of " + go);
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