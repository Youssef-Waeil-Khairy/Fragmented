
using EchoMina.Original;
using JetBrains.Annotations;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractorMoveable : MonoBehaviour, IInteractable, IRecordable
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform startingTransform;
        [SerializeField] private Vector3 startingPosition;
        [SerializeField] private Quaternion startingRotation;
        [SerializeField] private bool isPickedUp = false;
        [SerializeField] private Outline outline;
        private Interactor _interactor;
        [SerializeField] private bool isLocked = false;

        [BoxGroup("Snapshot")] [SerializeField]
        private Transform snapshotParentTransform;

        [BoxGroup("Snapshot")] [SerializeField]
        private Transform snapshotTransform;

        void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError($"<b><color=red>[Start Up][Moveable Object]</color.</b> {gameObject.name} does not have a RigidBody component.");
            }
        }

        #region Private Functions

        private void PickUpObject(Interactor interactor)
        {
            Debug.Log($"<b><color=green>[Interactions][Moveable Object]</color></b> {gameObject.name} has been picked up by {interactor.gameObject.name}");
            Lock(interactor);
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;

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
            _rigidbody.isKinematic = false;

            transform.position = interactor.DetachingTransform.position;
            transform.rotation = interactor.DetachingTransform.rotation;
            transform.SetParent(startingTransform);

            Unlock(interactor);
        }

        [Button][UsedImplicitly]
        private void EchoDrop()
        {
            DropObject(OriginalEchoMina.Instance.EchoInteractor);
        }

        [Button][UsedImplicitly]
        private void MinaDrop()
        {
            DropObject(PlayerController.Instance.gameObject.GetComponent<Interactor>());
        }

        #endregion

        #region IInteractable

        public bool CanInteract(Interactor interactor)
        {
            // if we are not picked up, we can be picked up
            if (!isPickedUp)
            {
                return true;
            }
            
            // if we are picked up the interactor needs to be the same as our current one
            if (isPickedUp && interactor == _interactor)
            {
                return true;
            }
            
            Debug.Log($"{gameObject.name} is already picked up by {_interactor.gameObject.name}");
            return false;
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
        public void StartPreview()
        {
            Debug.Log($"<b><color=green>[Interactions][Moveable Objects][Preview]</color></b> Preview started for {gameObject.name}");
            outline.enabled = true;
        }
        public void StopPreview()
        {
            Debug.Log($"<b><color=red>[Interactions][Moveable Objects][Preview]</color></b> Preview ended for {gameObject.name}");
            if (outline) outline.enabled = false;
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
        
        #region IRecordable

        public void TakeSnapshot()
        {
            snapshotParentTransform = transform.parent;
            snapshotTransform = transform;
        }
        public void LoadSnapshot()
        {
            transform.SetParent(snapshotParentTransform);
            transform.position = snapshotTransform.position;
            transform.rotation = snapshotTransform.rotation;
        }

  #endregion
    }
}