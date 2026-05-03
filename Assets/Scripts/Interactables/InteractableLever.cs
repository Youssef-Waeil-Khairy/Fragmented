using System;
using EchoMina.Original;
using NaughtyAttributes;
using PlayerControls;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Interactables
{
    public class InteractableLever : MonoBehaviour, IInteractable, IBindable
    {

        [SerializeField] private bool isInteracted;
        [SerializeField] private GameObject previewPanel;
        [SerializeField] private bool hasBeenInteracted = false;

        [Foldout("Events")][SerializeField] private UnityEvent onInteract;
        [Foldout("Events")][SerializeField] private UnityEvent onUnInteract;
        [Foldout("Events")][SerializeField] private UnityEvent onStartPreview;
        [Foldout("Events")][SerializeField] private UnityEvent onStopPreview;

        [Foldout("Camera")][SerializeField] private bool hasInteractionCamera = false;
        [Foldout("Camera")][SerializeField] private CinemachineCamera interactionCamera;
        [Foldout("Camera")][SerializeField] private float cameraDuration;
        [Foldout("Camera")][SerializeField] private float cameraTime;

        private void Awake()
        {
            hasInteractionCamera = interactionCamera != null;
            if (hasInteractionCamera) interactionCamera.enabled = false;
            if (previewPanel != null) previewPanel.SetActive(false);
        }

        private void Update()
        {
            if (!hasInteractionCamera) return;

            if (interactionCamera.enabled)
            {
                cameraTime += Time.deltaTime;

                if (cameraTime >= cameraDuration)
                {
                    HideInteraction();
                }
            }
        }

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
            if (hasBeenInteracted == false)
            {
                ShowInteraction();
                hasBeenInteracted = true;
            }

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
            if (previewPanel != null)
            {
                previewPanel.SetActive(true);
            }
            onStartPreview?.Invoke();
        }
        public void StopPreview()
        {
            if (previewPanel != null)
            {
                previewPanel.SetActive(false);
            }
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
        public bool ShouldShowInteraction()
        {
            return hasBeenInteracted == false;
        }
        public void ShowInteraction()
        {
            if (!hasInteractionCamera) return;

            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
            {
                // Disable Echo  Mina's camera
                OriginalEchoMina.Instance.EchoCamera.enabled = false;
            }
            else
            {
                // Disable the player's camera and input
                PlayerController.Instance.PlayerCamera.enabled = false;
            }

            PlayerController.Instance.PlayerInput.enabled = false;
            interactionCamera.enabled = true;
        }
        public void HideInteraction()
        {
            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
            {
                OriginalEchoMina.Instance.EchoCamera.enabled = true;
            }
            else
            {
                PlayerController.Instance.PlayerCamera.enabled = true;
            }

            PlayerController.Instance.PlayerInput.enabled = true;
            interactionCamera.enabled = false;
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