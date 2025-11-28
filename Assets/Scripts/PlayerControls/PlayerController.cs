using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
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
        [SerializeField] private bool cursorFree = false;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            characterController = GetComponent<CharacterController>();
            LockCursor();
        }

        private void FixedUpdate()
        {
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
            moveDirection = inputDirection.y;
            turnDirection = inputDirection.x;
        }

        void OnSprint(InputValue value)
        {
            isSprinting = value.isPressed;
        }
    }
}