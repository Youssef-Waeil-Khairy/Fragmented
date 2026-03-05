using System;
using System.Collections.Generic;
using EchoMina.Original;
using Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    [RequireComponent(typeof(BoxCollider))]
    public class Interactor : MonoBehaviour, IBindable
    {
        [ShowNonSerializedField] private IInteractable _interactable;

        [Foldout("Moveable Transforms")] public Transform AttachingTransform;
        [Foldout("Moveable Transforms")] public Transform DetachingTransform;

        public bool IsMina = false;
        [Foldout("Echo Mina")][SerializeField] private float _timeSinceLastInteraction = 0f;
        [Foldout("Echo Mina")][SerializeField] private OriginalEchoMina _originalEchoMina;
        [Foldout("Echo Mina")][SerializeField] private bool isRecording = false;
        
        // TODO: Check if we are holding something before allowing an interaction

        #region Unity Functions
        void Update()
        {
            if (isRecording && IsMina) _timeSinceLastInteraction += Time.deltaTime;
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
                    Debug.Log($"<b><color=green>[Interactions][Interactor]</color></b> {gameObject.name} Entered interaction range of " + go);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            var go = other.GetComponent<IInteractable>();
            if (go != null)
            {
                if (go == _interactable)
                {
                    if (_interactable.IsLockable() && _interactable.IsLocked())
                    {
                        Debug.Log($"<b><color=yellow>[Interactions][Locking]</color></b> {other.gameObject.name} is locked to {gameObject.name} and is now outside interaction range");
                        return;
                    }
                    
                    go.StopPreview();
                    _interactable = null;
                    Debug.Log($"<b><color=red>[Interactions][Interactor]</color></b> {gameObject.name} Exited interaction range of " + go);
                }
            }
        }

        private void OnDisable()
        {
            if (_interactable != null)
            {
                _interactable?.StopPreview();
                _interactable = null;
            }
        }

  #endregion

        #region Public Functions

        public void OnInteract(InputValue context)
        {
            if (IsMina && OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
            {
                OriginalEchoMina.Instance.SaveInteraction(_timeSinceLastInteraction);
                _timeSinceLastInteraction = 0f;
                OriginalEchoMina.Instance.EchoInteractor.InteractCommand();
                return;
            }
            
            if (_interactable != null && _interactable.CanInteract(this))
            {
                _interactable.Interact(this);
            }
        }

        public void InteractCommand()
        {
            Debug.Log("<b><color=green>[Interactions][Interactor][Echo Mina]</color></b> Interaction Command received");
            if (_interactable != null && _interactable.CanInteract(this))
            {
                _interactable.Interact(this);
            }
        }

        public void StartRecording()
        {
            _timeSinceLastInteraction = 0f;
        }

        public void UnlockObject()
        {
            _interactable.StopPreview();
            _interactable = null;
        }

  #endregion
        #region IBindable

        public void BindObject()
        {
            if (IsMina)
            {
                OriginalEchoMina.Instance.StartRecording += StartRecording;
            }
        }
        public void UnBindObject()
        {
            if (IsMina)
            {
                OriginalEchoMina.Instance.StartRecording -= StartRecording;
            }
        }

  #endregion
    }
}