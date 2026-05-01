using EchoMina.Original;
using JetBrains.Annotations;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private GameObject previewPanel;
        private Interactor _interactor;
        [SerializeField] private bool isLocked = false;

        [BoxGroup("Snapshot")][SerializeField] private Transform snapshotParentTransform;
        [BoxGroup("Snapshot")][SerializeField] private Transform snapshotTransform;
        [BoxGroup("Snapshot")][SerializeField] private Transform checkpointSnapshotParentTransform;
        [BoxGroup("Snapshot")][SerializeField] private Transform checkpointSnapshotTransform;

        // ✅ Events — subscribe to these from other scripts or wire up in Inspector
        [Foldout("Events")][SerializeField] private UnityEvent onPickedUp;
        [Foldout("Events")][SerializeField] private UnityEvent onDropped;
        [Foldout("Events")][SerializeField] private UnityEvent onPreviewStarted;
        [Foldout("Events")][SerializeField] private UnityEvent onPreviewStopped;
        [Foldout("Events")][SerializeField] private UnityEvent onLocked;
        [Foldout("Events")][SerializeField] private UnityEvent onUnlocked;

        // ✅ C# delegates — subscribe to these from code
        public delegate void OnPickedUpDelegate(Interactor interactor);
        public delegate void OnDroppedDelegate(Interactor interactor);
        public delegate void OnPreviewStartedDelegate();
        public delegate void OnPreviewStoppedDelegate();
        public delegate void OnLockedDelegate(Interactor interactor);
        public delegate void OnUnlockedDelegate(Interactor interactor);

        public event OnPickedUpDelegate PickedUp;
        public event OnDroppedDelegate Dropped;
        public event OnPreviewStartedDelegate PreviewStarted;
        public event OnPreviewStoppedDelegate PreviewStopped;
        public event OnLockedDelegate Locked;
        public event OnUnlockedDelegate Unlocked;

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
            StopPreview();

            // ✅ Fire pick up events
            onPickedUp?.Invoke();
            PickedUp?.Invoke(interactor);
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
            StartPreview();

            // ✅ Fire drop events
            onDropped?.Invoke();
            Dropped?.Invoke(interactor);
        }

        [Button]
        [UsedImplicitly]
        private void EchoDrop()
        {
            DropObject(OriginalEchoMina.Instance.EchoInteractor);
        }

        [Button]
        [UsedImplicitly]
        private void MinaDrop()
        {
            DropObject(PlayerController.Instance.gameObject.GetComponent<Interactor>());
        }

        #endregion

        #region IInteractable

        public bool CanInteract(Interactor interactor)
        {
            if (!isPickedUp) return true;

            if (isPickedUp && interactor == _interactor) return true;

            Debug.Log($"{gameObject.name} is already picked up by {_interactor.gameObject.name}");
            return false;
        }

        public void Interact(Interactor interactor)
        {
            if (isPickedUp)
            {
                DropObject(interactor);
                Debug.Log($"{gameObject.name} is dropped by {interactor.gameObject.name}");
            }
            else
            {
                PickUpObject(interactor);
                Debug.Log($"{gameObject.name} is picked up by {interactor.gameObject.name}");
            }
        }

        public void StartPreview()
        {
            Debug.Log($"<b><color=green>[Interactions][Moveable Objects][Preview]</color></b> Preview started for {gameObject.name}");
            outline.enabled = true;
            previewPanel.SetActive(true);

            // ✅ Fire preview started events
            onPreviewStarted?.Invoke();
            PreviewStarted?.Invoke();
        }

        public void StopPreview()
        {
            Debug.Log($"<b><color=red>[Interactions][Moveable Objects][Preview]</color></b> Preview ended for {gameObject.name}");
            if (outline) outline.enabled = false;
            previewPanel.SetActive(false);

            // ✅ Fire preview stopped events
            onPreviewStopped?.Invoke();
            PreviewStopped?.Invoke();
        }

        public bool IsLockable() => true;
        public bool IsLocked() => isLocked;

        public void Lock(Interactor interactor)
        {
            _interactor = interactor;
            isLocked = true;
            Debug.Log($"<b><color=green>[Interactions][Lock]</color></b> {gameObject.name} is locked to {interactor.name}");

            // ✅ Fire lock events
            onLocked?.Invoke();
            Locked?.Invoke(interactor);
        }

        public void Unlock(Interactor interactor)
        {
            Debug.Log($"<b><color=red>[Interactions][Lock]</color></b> {gameObject.name} is no longer locked to {interactor.name}");
            interactor.UnlockObject();
            _interactor = null;
            isLocked = false;

            // ✅ Fire unlock events
            onUnlocked?.Invoke();
            Unlocked?.Invoke(interactor);
        }

        public void ShowInteraction() => Debug.Log("Should be unreachable");
        public void HideInteraction() => Debug.Log("Should be unreachable");

        #endregion

        #region IRecordable

        public void TakeSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Take]</color></b> Snapshot: {gameObject.name}]");

            snapshotParentTransform = transform.parent;
            snapshotTransform = transform;
            startingPosition = transform.position;
            startingRotation = transform.rotation;
        }
        public void TakeCheckpointSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Take][Checkpoint]</color></b> Snapshot: {gameObject.name}]");

            checkpointSnapshotParentTransform = transform.parent;
            checkpointSnapshotTransform = transform;
        }
        public void LoadSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Load]</color></b> Snapshot: {gameObject.name}]");

            transform.SetParent(snapshotParentTransform);
            transform.position = startingPosition;
            transform.rotation = startingRotation;
        }
        public void LoadCheckpointSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Load][Checkpoint]</color></b> Snapshot: {gameObject.name}]");

            transform.SetParent(checkpointSnapshotParentTransform);
            transform.position = checkpointSnapshotTransform.position;
            transform.rotation = checkpointSnapshotTransform.rotation;
        }

        #endregion
    }
}