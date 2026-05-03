using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using EchoMina.Original;
using Interactables;
using JetBrains.Annotations;
using MainMenu;
using NaughtyAttributes;
using QuickLoad;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utility;

namespace PlayerControls
{
    [RequireComponent(typeof(PlayerInput))]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    public class PlayerController : MonoBehaviour, IRecordable
    {
        public static bool PLAYERCONTROLLERSTARTED = false;

        public static PlayerController Instance;

        PlayerInput playerInput;
        //CharacterController characterController;
        private Rigidbody rb;

        public bool EchoMinaCooldownActive = false;
        public float EchoMinaCooldown = 3f;

        [Foldout("Movement")][SerializeField] float moveSpeed = 5f;
        [Foldout("Movement")][SerializeField] float turnSpeed = 5f;
        [Foldout("Movement")][SerializeField] float moveDirection = 0f;
        [Foldout("Movement")][SerializeField] float turnDirection = 0f;
        [Foldout("Movement")][SerializeField] private float maxSpeed;
        [Foldout("Movement")][SerializeField] private float maxTurnSpeed;
        [Foldout("Movement")][SerializeField] private PhysicsMaterial controlablesMaterial;
        [Foldout("Movement")][SerializeField] private float moveInput;

        [Foldout("Camera")][SerializeField] private CinemachineInputAxisController cinemachineInputAxisController;
        [Foldout("Camera")][SerializeField] private CinemachineCamera playerCamera;
        [Foldout("Camera")][SerializeField] private bool cursorFree = false;

        [Foldout("Snapshot")][SerializeField] private Vector3 snapshotPosition;
        [Foldout("Snapshot")][SerializeField] private Quaternion snapshotRotation;
        [Foldout("Snapshot")][SerializeField] private Vector3 checkpointSnapshotPosition;
        [Foldout("Snapshot")][SerializeField] private Quaternion checkpointSnapshotRotation;

        // Getters
        public CinemachineCamera PlayerCamera => playerCamera;
        public PlayerInput PlayerInput => playerInput;

        //getters for animtion
        public float MoveDirection => moveDirection;


        public void Start()
        {
            StartupLogger.LogStart("Setting up PlayerController Singleton", name);
            if (Instance == null)
            {
                StartupLogger.LogStart($"PlayerController Singleton instance was null, setting it to {gameObject.name}'s PlayerController", name);
                Instance = this;
            }
            else if (Instance != this)
            {
                StartupLogger.LogStart($"PlayerController Singleton instance already exists and is not us. Destroying {gameObject.name}", name);

                Destroy(gameObject);
            }

            playerInput = GetComponent<PlayerInput>();
            rb  = GetComponent<Rigidbody>();
            rb.maxLinearVelocity = maxSpeed;
            rb.maxAngularVelocity = maxTurnSpeed;
            LockCursor();

            OriginalEchoMina.Instance.StopRecording -= OnEchoMinaRecordingStopped;
            OriginalEchoMina.Instance.StopRecording += OnEchoMinaRecordingStopped;
            StartupLogger.LogEnable("Player Echo Mina recording stopped event subscribed", "Player Controller");

            PLAYERCONTROLLERSTARTED = true;
            StartupLogger.LogStart("Finished start up successfully", name);
        }

        private void OnEnable()
        {
            EchoMinaCooldownActive = false;

            ScreenTransitioner.Instance.EndFadeOut -= RestoreSnapshotAfterReturn; // Trust no one to remove themself (make sure we aren't already subscribed somehow to avoid double firing it)
            ScreenTransitioner.Instance.EndFadeOut += RestoreSnapshotAfterReturn;
            StartupLogger.LogEnable("Player position restoring subscribed", "Player Controller");
        }

        void OnDisable()
        {
            ScreenTransitioner.Instance.EndDelayOut -= RestoreSnapshotAfterReturn;
            OriginalEchoMina.Instance.StopRecording -= OnEchoMinaRecordingStopped;
        }

        private void OnEchoMinaRecordingStopped()
        {
            StartCoroutine(StartEchoMinaCooldown());
        }

        private void RestoreSnapshotAfterReturn()
        {
            if (PauseMenu.Instance == null)
            {
                Debug.LogWarning("Position restoring of the player expects pause menu but none found");
                return;
            }
            if (!PauseMenu.Instance.ShouldLoadSnapShot)
            {
                return;
            }
            if (this == null)
            {
                Debug.LogWarning("this is null");
                return;
            }

            Debug.LogWarning($"Before restoring: Snapshot position: {PauseMenu.Instance.CurrentPosition} | Current position: {transform.position}");

            Debug.LogWarning("Player loading pause menu snapshot");
            transform.SetPositionAndRotation(
                PauseMenu.Instance.CurrentPosition,
                PauseMenu.Instance.CurrentRotation
                );

            Debug.LogWarning($"After restoring: Snapshot position: {PauseMenu.Instance.CurrentPosition} | Current position: {transform.position}");

            PauseMenu.Instance.ShouldLoadSnapShot = false;
        }



        private void FixedUpdate()
        {
            if (moveInput != 0)
            {
                controlablesMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            }
            else
            {
                controlablesMaterial.frictionCombine = PhysicsMaterialCombine.Average;
            }

            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording || cursorFree)
            {
                if (cursorFree)
                {
                    cinemachineInputAxisController.enabled = false;
                }
                return;
            }

            Vector3 move = transform.forward * (moveDirection * moveSpeed);
            rb.AddForce(move, ForceMode.Force);
            rb.AddTorque(transform.up * (turnDirection * turnSpeed * Time.fixedDeltaTime));
        }

        private IEnumerator StartEchoMinaCooldown()
        {
            EchoMinaCooldownActive = true;
            yield return new WaitForSeconds(EchoMinaCooldown);
            EchoMinaCooldownActive = false;
            yield break;
        }

        void ToggleCursorFree()
        {
            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording) return;

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

        /// <summary>
        /// Resume camera movement and hides and locks the cursor
        /// </summary>
        public void LockCursor()
        {
            cursorFree = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cinemachineInputAxisController.enabled = true;
            Debug.Log("Locking cursor.");
        }

        /// <summary>
        /// Stops camera movement and shows and confines the cursor
        /// </summary>
        public void UnlockCursor()
        {
            cursorFree = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            cinemachineInputAxisController.enabled = false;
            Debug.Log("Unlocking cursor.");
        }

        [UsedImplicitly]
        void OnFreeCursor(InputValue value)
        {
            if (value.isPressed)
            {
                ToggleCursorFree();
            }
        }

        [UsedImplicitly]
        void OnMove(InputValue value)
        {
            if (cursorFree) return;

            Vector2 inputDirection = value.Get<Vector2>();
            moveInput = inputDirection.y;
            // Block move inputs to main Mina if we are recording
            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
            {
                Vector3 recordInput = new Vector3(inputDirection.y, inputDirection.x, 0f);
                OriginalEchoMina.Instance.OnMove(recordInput);
                return;
            }

            moveDirection = inputDirection.y;
            turnDirection = inputDirection.x;
        }

        [UsedImplicitly]
        void OnMinaRecord(InputValue value)
        {
            if (EchoMinaCooldownActive)
            {
                Debug.LogWarning("Echo Mina cooldown is still active");
                return;
            }

            ToggleRecording();
        }

        public void ToggleRecording()
        {
            if (cursorFree) return;

            //characterController.enabled = !characterController.enabled;
            //Debug.Log($"position: {transform.position}, rotation: {transform.rotation}");

            OriginalEchoMina.Instance.transform.position = transform.position;
            OriginalEchoMina.Instance.transform.rotation = transform.rotation;

            OriginalEchoMina.Instance.ToggleRecording();

            if (OriginalEchoMina.Instance.State is not OriginalEchoMina.EchoState.Recording)
            {
                playerCamera.enabled = true;
                cinemachineInputAxisController.enabled = true;
            }
            else
            {
                playerCamera.enabled = false;
                cinemachineInputAxisController.enabled = false;
            }
        }

        [UsedImplicitly]
        void OnMinaPlayback(InputValue value)
        {
            if (cursorFree) return;

            OriginalEchoMina.Instance.TogglePlaying();
            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Playing)
            {
                playerCamera.enabled = true;
                cinemachineInputAxisController.enabled = true;
            }
        }

        [UsedImplicitly]
        void OnLoadCheckpoint(InputValue value)
        {
            if (cursorFree) return;

            if (value.isPressed)
            {
                QuickLoader.Instance.LoadCheckpoint();
            }
        }

        [UsedImplicitly]
        void OnQuickLoad(InputValue value)
        {
            if (cursorFree) return;

            if (value.isPressed)
            {
                Debug.Log("Quick Load");
                QuickLoader.Instance.QuickLoadCheckpoint();
            }
        }

        [UsedImplicitly]
        void OnQuickSave(InputValue value)
        {
            if (cursorFree) return;

            if (value.isPressed)
            {
                Debug.Log("Quick Save");
                QuickLoader.Instance.SaveCheckpoint();
            }
        }
        public void TakeSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Take]</color></b> Snapshot: {gameObject.name}]");

            snapshotPosition = transform.position;
            snapshotRotation = transform.rotation;
        }
        public void TakeCheckpointSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Take][Checkpoint]</color></b> Snapshot: {gameObject.name}]");

            checkpointSnapshotPosition = transform.position;
            checkpointSnapshotRotation = transform.rotation;
        }
        public void LoadSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Load]</color></b> Snapshot: {gameObject.name}]");

            transform.position = snapshotPosition;
            snapshotPosition.y += 1f;
            transform.rotation = snapshotRotation;
        }
        public void LoadCheckpointSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Load][Checkpoint]</color></b> Snapshot: {gameObject.name}]");

            transform.position = checkpointSnapshotPosition;
            transform.rotation = checkpointSnapshotRotation;
        }

        [Button]
        public void SetMaxSpeed()
        {
            rb.maxLinearVelocity = maxSpeed;
            rb.maxAngularVelocity = maxTurnSpeed;
        }

        public void ToggleInputEnabled(bool isEnabled)
        {
            if (playerInput != null)
            {
                playerInput.enabled = isEnabled;
            }
        }
    }
}