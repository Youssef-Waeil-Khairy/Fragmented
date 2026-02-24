using System;
using EchoMina.Original;
using Interactables;
using JetBrains.Annotations;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        public static bool HASSTARTEDUP = false;
        
        PlayerInput playerInput;
        CharacterController characterController;

        [Header("Movement")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float turnSpeed = 5f;
        [SerializeField] float sprintSpeed = 10f;
        [SerializeField] float moveDirection = 0f;
        [SerializeField] float turnDirection = 0f;
        [SerializeField] bool isSprinting = false;

        [Header("Camera")]
        [SerializeField] private CinemachineInputAxisController cinemachineInputAxisController;
        [SerializeField] private CinemachineCamera playerCamera;
        [SerializeField] private bool cursorFree = false;
        
        [Header("Echo Mina")]
        [SerializeField] private OriginalEchoMina originalEchoMina;
        
        public OriginalEchoMina EchoMina => originalEchoMina;

        public void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            
            playerInput = GetComponent<PlayerInput>();
            characterController = GetComponent<CharacterController>();
            LockCursor();
            
            HASSTARTEDUP = true;
        }

        private void FixedUpdate()
        {
            if (!HASSTARTEDUP) return;
            
            if (originalEchoMina.IsRecording)
            {
                return;
            }
            Vector3 move = transform.forward * (moveDirection * (isSprinting ? sprintSpeed : moveSpeed));
            characterController.SimpleMove(move);
            transform.Rotate(transform.up, turnDirection *  turnSpeed * Time.fixedDeltaTime);
        }

        void ToggleCursorFree()
        {
            cursorFree = !cursorFree;

            if (cursorFree)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }

        void LockCursor()
        {
            cursorFree = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cinemachineInputAxisController.enabled = true;
        }
        void UnlockCursor()
        {
            cursorFree = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            cinemachineInputAxisController.enabled = false;
        }

        void OnFreeCursor(InputValue value)
        {
            if (value.isPressed)
            {
                ToggleCursorFree();
            }
        }

        void OnMove(InputValue value)
        {
            Vector2 inputDirection = value.Get<Vector2>();
            
            // Block move inputs to main Mina if we are recording
            if (originalEchoMina.IsRecording)
            {
                Vector3 recordInput = new Vector3(inputDirection.y, inputDirection.x, isSprinting ? 1f : 0f);
                originalEchoMina.OnMove(recordInput);
                return;
            }
            
            moveDirection = inputDirection.y;
            turnDirection = inputDirection.x;
        }

        void OnSprint(InputValue value)
        {
            isSprinting = value.isPressed;
        }

        [UsedImplicitly]
        void OnMinaRecord(InputValue value)
        {
            //characterController.enabled = !characterController.enabled;
            Debug.Log($"position: {transform.position}, rotation: {transform.rotation}");
            originalEchoMina.ToggleRecording(transform.position, transform.rotation);
            if (!originalEchoMina.IsRecording)
            {
                GetComponent<Interactor>().RecordingStopped();
                playerCamera.enabled = true;
                cinemachineInputAxisController.enabled = true;
            }
            else
            {
                GetComponent<Interactor>().RecordingStarted();
                playerCamera.enabled = false;
                cinemachineInputAxisController.enabled = false;
            }
        }

        [UsedImplicitly]
        void OnMinaPlayback(InputValue value)
        {
            originalEchoMina.TogglePlaying();
        }
    }
}