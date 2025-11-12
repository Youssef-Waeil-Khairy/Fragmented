using System;
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
        
        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            characterController = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            Vector3 move = transform.forward * (moveDirection * (isSprinting ? sprintSpeed : moveSpeed));
            characterController.SimpleMove(move);
            transform.Rotate(transform.up, turnDirection *  turnSpeed * Time.fixedDeltaTime);
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