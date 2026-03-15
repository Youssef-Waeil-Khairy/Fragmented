using System.Diagnostics.CodeAnalysis;
using EchoMina.Original;
using Interactables;
using JetBrains.Annotations;
using NaughtyAttributes;
using QuickLoad;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    [RequireComponent(typeof(PlayerInput))]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    public class PlayerController : MonoBehaviour, IRecordable
    {
        public static PlayerController Instance;

        //PlayerInput playerInput;
        //CharacterController characterController;
        private Rigidbody rb;

        [Foldout("Movement")][SerializeField] float moveSpeed = 5f;
        [Foldout("Movement")][SerializeField] float turnSpeed = 5f;
        [Foldout("Movement")][SerializeField] float moveDirection = 0f;
        [Foldout("Movement")][SerializeField] float turnDirection = 0f;
        [Foldout("Movement")][SerializeField] private float maxSpeed;
        [Foldout("Movement")] [SerializeField] private float maxTurnSpeed;

        [Foldout("Camera")][SerializeField] private CinemachineInputAxisController cinemachineInputAxisController;
        [Foldout("Camera")][SerializeField] private CinemachineCamera playerCamera;
        [Foldout("Camera")][SerializeField] private bool cursorFree = false;

        [Foldout("Snapshot")][SerializeField] private Vector3 snapshotPosition;
        [Foldout("Snapshot")][SerializeField] private Quaternion snapshotRotation;

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

            //playerInput = GetComponent<PlayerInput>();
            rb  = GetComponent<Rigidbody>();
            rb.maxLinearVelocity = maxSpeed;
            rb.maxAngularVelocity = maxTurnSpeed;
            LockCursor();
        }

        private void FixedUpdate()
        {
            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
            {
                return;
            }

            Vector3 move = transform.forward * (moveDirection * moveSpeed);
            rb.AddForce(move, ForceMode.Force);
            rb.AddTorque(transform.up * (turnDirection * turnSpeed * Time.fixedDeltaTime));

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
            Vector2 inputDirection = value.Get<Vector2>();

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
            if (cursorFree) return;

            //characterController.enabled = !characterController.enabled;
            Debug.Log($"position: {transform.position}, rotation: {transform.rotation}");
            OriginalEchoMina.Instance.ToggleRecording(transform.position, transform.rotation);
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
            if (value.isPressed)
            {
                QuickLoader.Instance.LoadCheckpoint();
            }
        }

        [UsedImplicitly]
        void OnQuickLoad(InputValue value)
        {
            if (value.isPressed)
            {
                Debug.Log("Quick Load");
                QuickLoader.Instance.QuickLoadCheckpoint();
            }
        }

        [UsedImplicitly]
        void OnQuickSave(InputValue value)
        {
            if (value.isPressed)
            {
                Debug.Log("Quick Save");
                QuickLoader.Instance.SaveCheckpoint();
            }
        }
        public void TakeSnapshot()
        {
            snapshotPosition = transform.position;
            snapshotRotation = transform.rotation;
        }
        public void LoadSnapshot()
        {
            transform.position = snapshotPosition;
            transform.rotation = snapshotRotation;
        }

        [Button]
        public void SetMaxSpeed()
        {
            rb.maxLinearVelocity = maxSpeed;
            rb.maxAngularVelocity = maxTurnSpeed;
        }
    }
}