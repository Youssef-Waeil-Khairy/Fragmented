using System;
using System.Collections;
using System.Collections.Generic;
using EchoMina.Original;
using NaughtyAttributes;
using PlayerControls;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Utility;
// ReSharper disable InconsistentNaming

namespace Interactables
{
    public class InteractorPressurePlate : MonoBehaviour, IBindable
    {
        [SerializeField] private bool _isInteracting = false;
        [Tag][SerializeField] private List<string> _whitelistTags = new List<string>() {"Player"};
        [SerializeField] private GameObject _interactingObject;
        public UnityEvent OnBeginInteraction, OnEndInteraction;

        [Foldout("Camera")]
        [InfoBox("Make sure the interaction camera has a higher priority than the player and Echo Mina")]
        /*still in the foldout  */[SerializeField] private bool _hasInteractionCamera = false;
        [Foldout("Camera")] [SerializeField] private CinemachineCamera _camera;
        [Foldout("Camera")] [SerializeField] private float _cameraDuration;
        [Foldout("Camera")] [SerializeField] private float _cameraTime;
        [Foldout("Camera")] [SerializeField] private bool  _hasBeenInteracted = false;

        private void Awake()
        {
            _hasInteractionCamera = _camera != null;
            if (_hasInteractionCamera) _camera.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_whitelistTags.Contains(other.tag) && !_isInteracting)
            {
                #if DEBUG
                Debug.Log($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped on by {other.gameObject.name}");
                #endif

                _interactingObject = other.gameObject;

                Interact();
            }
        }

        [Button("Interact")]
        public void Interact()
        {
            OnBeginInteraction?.Invoke();
            _isInteracting = true;

            if (!_hasBeenInteracted && _hasInteractionCamera)
            {
                StartCoroutine(ShowInteractionRoutine());
            }
        }
        [Button]
        public void ResetInteractions()
        {
            _hasBeenInteracted = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isInteracting && other.gameObject == _interactingObject)
            {
                EndInteraction();
            }
        }

        [Button]
        private void EndInteraction()
        {
            #if DEBUG
            if (_interactingObject) Debug.Log($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped off by {_interactingObject.name}");
            #endif

            if (_interactingObject) _interactingObject =  null;
            OnEndInteraction?.Invoke();
            _isInteracting = false;
        }

        [Button]
        public void StartShowInteraction()
        {
            Debug.Log($"Showing Interaction camera for {gameObject.name}");
            StartCoroutine(ShowInteractionRoutine());
        }

        private IEnumerator ShowInteractionRoutine()
        {
            if (_camera == null)
            {
                Debug.Log("No Camera to show");
                yield return null;
            }

            PlayerController.Instance.PlayerInput.enabled = false;

            _camera.enabled = true;

            yield return new WaitForSeconds(_cameraDuration);

            _camera.enabled = false;

            PlayerController.Instance.PlayerInput.enabled = true;
            _hasBeenInteracted = true;
            yield break;
        }

        private void RecordingStopped()
        {
            if (_isInteracting)
            {
                if (_interactingObject.CompareTag("EchoMina"))
                {
                    Debug.LogWarning("Pressure plate is being interacted with by the Echo Mina when recording stopped");
                    EndInteraction();
                }
            }
        }

        private void PlaybackStopped()
        {
            if (!_isInteracting) return;

            EndInteraction();
        }
        public void BindObject()
        {
            OriginalEchoMina.Instance.StopPlayback += PlaybackStopped;
            OriginalEchoMina.Instance.StopRecording += RecordingStopped;
        }
        public void UnBindObject()
        {
            OriginalEchoMina.Instance.StopPlayback -= PlaybackStopped;
            OriginalEchoMina.Instance.StopRecording -= RecordingStopped;
        }
    }
}