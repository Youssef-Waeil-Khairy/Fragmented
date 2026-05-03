using System;
using EchoMina.Original;
using NaughtyAttributes;
using PlayerControls;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
// ReSharper disable InconsistentNaming

namespace Interactables
{
    /// <summary>
    /// Author: Jan Willem Goedvolk
    /// A button like interactable
    /// </summary>
    public class InteractorSingleFire : MonoBehaviour,  IInteractable
    {
        #region Variables
        [Foldout("Events" )] public UnityEvent OnInteract, OnPreviewStart,  OnPreviewEnd;

        [Foldout("Preview")] public Outline PreviewOutline;
        [Foldout("Preview")] public GameObject PreviewPanel;

        [Foldout("Camera")][SerializeField] private CinemachineCamera interactionCamera;
        [Foldout("Camera")][SerializeField] private float cameraDuration;
        [Foldout("Camera")][SerializeField] private float cameraTime;
        [Foldout("Camera")][SerializeField] private bool hasBeenInteracted = false;
        [Foldout("Camera")][SerializeField] private bool hasInteractionCamera = false;
        #endregion

        #region Unity Functions
        private void OnEnable()
        {
            PreviewOutline = GetComponent<Outline>();
            if (PreviewOutline != null) PreviewOutline.enabled = false;

            if (PreviewPanel != null) PreviewPanel.SetActive(false);

            hasInteractionCamera = interactionCamera != null;
            if (hasInteractionCamera) interactionCamera.enabled = false;
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
        #endregion

        #region IInteractable Functions
        public bool CanInteract(Interactor interactor)
        {
            return true;
        }
        public void Interact(Interactor interactor)
        {
            OnInteract?.Invoke();

            if (hasBeenInteracted == false)
            {
                ShowInteraction();
            }

            hasBeenInteracted = true;
        }
        public void StartPreview()
        {
            if (PreviewOutline != null) PreviewOutline.enabled = true;
            if (PreviewPanel   != null) PreviewPanel.SetActive(true);
            OnPreviewStart?.Invoke();

        }
        public void StopPreview()
        {
            if (PreviewOutline != null) PreviewOutline.enabled = false;
            if (PreviewPanel   != null) PreviewPanel.SetActive(false);
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
    }
}