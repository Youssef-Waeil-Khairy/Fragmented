using System.Collections;
using EchoMina.Original;
using JetBrains.Annotations;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractorMoveable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform startingTransform;
        [SerializeField] private Vector3 startingPosition;
        [SerializeField] private Quaternion startingRotation;
        [SerializeField] private bool isPickedUp = false;
        [SerializeField][ReadOnly] private bool wasPickedUp = false;
        [SerializeField] private Outline outline;
        [ShowNonSerializedField] private bool isRecording = false;
        private Interactor _interactor;
        [SerializeField] private bool isLocked = false;

        void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError($"<b><color=red>[Start Up][Moveable Object]</color.</b> {gameObject.name} does not have a RigidBody component.");
            }
        }

        private void OnDisable()
        {
            OriginalEchoMina.Instance.StartRecording -= RecordingStart;
            OriginalEchoMina.Instance.StopRecording -= RecordingEnd;
        }

        #region Public Functions

        public void BindObject()
        {
            OriginalEchoMina.Instance.StartRecording += RecordingStart;
            OriginalEchoMina.Instance.StopRecording += RecordingEnd;
        }
        public bool CanInteract(Interactor interactor)
        {
            if (isPickedUp)
            {
                return true; // TODO: Check the area we would place it in to make sure it is clear of any obstacles first
            }
            else
            {
                return true; // If we aren't picked up we can be picked up
            }
        }
        public void Interact(Interactor interactor)
        {
            if (isPickedUp)
            {
                DropObject(interactor);
            }
            else
            {
                PickUpObject(interactor);
            }
        }
        public void UnInteract(Interactor interactor)
        {
            if (wasPickedUp)
            {
                Debug.Log($"<b><color=purple>[Interactions][Moveable Objects]</color></b> {gameObject.name} has been un-interacted");
                
                if (isPickedUp)
                {
                    Debug.LogWarning($"<b><color=yellow>[Interactions][Moveable Object][Echo Mina Recording]</color></b> {gameObject.name} has been un-interacted while picked up");
                    DropObject(OriginalEchoMina.Instance.EchoInteractor);
                    
                    transform.position = startingPosition;
                    transform.rotation = startingRotation;
                    transform.SetParent(startingTransform);
                }
                else
                {
                    transform.position = startingPosition;
                    transform.rotation = startingRotation;
                    transform.SetParent(startingTransform);
                    
                    wasPickedUp = false;
                    isPickedUp = false;
                }
            }
        }
        public void StartPreview()
        {
            Debug.Log($"<b><color=green>[Interactions][Moveable Objects][Preview]</color></b> Preview started for {gameObject.name}");
            outline.enabled = true;
        }
        public void StopPreview()
        {
            Debug.Log($"<b><color=red>[Interactions][Moveable Objects][Preview]</color></b> Preview ended for {gameObject.name}");
            outline.enabled = false;
        }
        public bool IsLockable()
        {
            return true;
        }
        public bool IsLocked()
        {
            return isLocked;
        }
        public void Lock(Interactor interactor)
        {
            _interactor = interactor;
            isLocked = true;
            
            Debug.Log($"<b><color=green>[Interactions][Lock]</color></b> {gameObject.name} is locked to {interactor.name}");
        }
        
        public void Unlock(Interactor interactor)
        {
            Debug.Log($"<b><color=red>[Interactions][Lock]</color></b> {gameObject.name} is no longer locked to {interactor.name}");
            interactor.UnlockObject();
            _interactor = null;
            isLocked = false;
        }

  #endregion

        #region Private Functions

        private void ResetObject()
        {
            UnInteract(OriginalEchoMina.Instance.EchoInteractor);
        }

        private void RecordingStart()
        {
            isRecording = true;
        }

        private void RecordingEnd()
        {
            isRecording = false;

            if (wasPickedUp)
            {
                UnInteract(OriginalEchoMina.Instance.EchoInteractor);
            }
        }

        private void SnapshotObject()
        {
            startingTransform = transform.parent;
            startingPosition = transform.position;
            startingRotation = transform.rotation;
        }

        private void PickUpObject(Interactor interactor)
        {
            Debug.Log($"<b><color=green>[Interactions][Moveable Object]</color></b> {gameObject.name} has been picked up by {interactor.gameObject.name}");
            Lock(interactor);
            _rigidbody.useGravity = false;

            if (isRecording)
            {
                if (!wasPickedUp) // Only take a snapshot of the object if this object has not been picked up yet
                {
                    SnapshotObject();
                }
                
                wasPickedUp = true;
            }
            
            transform.position = interactor.AttachingTransform.position;
            transform.rotation = interactor.AttachingTransform.rotation;
            transform.SetParent(interactor.AttachingTransform);
            
            isPickedUp = true;
        }

        private void DropObject(Interactor interactor)
        {
            Debug.Log($"<b><color=red>[Moveable Objects][Interaction]</color></b> {gameObject.name} has been dropped by {interactor.gameObject.name}");
            isPickedUp = false;
            _rigidbody.useGravity = true;
                
            transform.position = interactor.DetachingTransform.position;
            transform.rotation = interactor.DetachingTransform.rotation;
            transform.SetParent(startingTransform);
            
            Unlock(interactor);
        }

        [Button]
        private void EchoDrop()
        {
            DropObject(OriginalEchoMina.Instance.EchoInteractor);
        }

        [Button]
        private void MinaDrop()
        {
            DropObject(PlayerController.Instance.gameObject.GetComponent<Interactor>());
        }

  #endregion
    }
}
