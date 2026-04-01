using System;
using System.Collections.Generic;
using EchoMina.Original;
using NaughtyAttributes;
using PlayerControls;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

        [Foldout("Camera")] [SerializeField] private bool _hasIntectionCaamera = false;
        [Foldout("Camera")] [SerializeField] private CinemachineCamera _camera;
        [Foldout("Camera")] [SerializeField] private float _cameraDuration;
        [Foldout("Camera")] [SerializeField] private float _cameraTime;
        [Foldout("Camera")] [SerializeField] private bool _hasBeenInteracted = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_whitelistTags.Contains(other.tag) && !_isInteracting)
            {
                #if DEBUG
                Debug.LogError($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped on by {other.gameObject.name}");
                #endif

                _interactingObject = other.gameObject;

                OnBeginInteraction?.Invoke();
                _isInteracting = true;

                if (!_hasBeenInteracted)
                {
                    if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
                    {
                        OriginalEchoMina.Instance.EchoCamera.enabled = false;
                    }
                    else
                    {
                        PlayerController.Instance.PlayerCamera.enabled = false;
                    }

                    PlayerController.Instance.PlayerInput.enabled = false;
                    _camera.enabled = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isInteracting && other.gameObject == _interactingObject)
            {
                #if DEBUG
                Debug.LogError($"<b><color=yellow>[Interactor][PressurePlate]</color></b> {gameObject.name} has been stepped off by {other.gameObject.name}");
                #endif

                _interactingObject =  null;
                OnEndInteraction?.Invoke();
                _isInteracting = false;
            }
        }

        private void Update()
        {
            if (!_hasIntectionCaamera) return;

            if (_camera.enabled)
            {
                _cameraTime += Time.deltaTime;

                if (_cameraTime >= _cameraDuration)
                {
                    _camera.enabled = false;

                    if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
                    {
                        OriginalEchoMina.Instance.EchoCamera.enabled = true;
                    }
                    else
                    {
                        PlayerController.Instance.PlayerCamera.enabled = true;
                    }

                    PlayerController.Instance.PlayerInput.enabled = true;
                    _camera.enabled = false;
                }
            }
        }

        private void RecordingStopped()
        {
            if (_isInteracting)
            {
                if (_interactingObject.CompareTag("Player"))
                {
                    Debug.LogError("Pressure plate is being interacted with by the player when recording stopped");
                    return;
                }
                else
                {
                    Debug.LogError("Pressure plate is being interacted with by the Echo Mins when recording stopped");
                    _isInteracting = false;
                    _interactingObject = null;
                    // BUG: [Pressure Plate] might need to call OnEndInteraction here as well
                }
            }
        }

        private void PlaybackStopped()
        {
            if (!_isInteracting) return;

            OnEndInteraction?.Invoke();
            _interactingObject = null;
            _isInteracting = false;
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