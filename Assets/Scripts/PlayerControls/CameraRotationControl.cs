using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    public class CameraRotationControl : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineInputAxisController inputAxisController;
        [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

        [Header("Settings")]
        [SerializeField] private int mouseButton = 0; // 0 = Left Click
        [SerializeField] private bool hideCursorWhileRotating = false;

        [Header("Rotation Limits")]
        [SerializeField] private bool allowHorizontalRotation = true;
        [SerializeField] private bool allowVerticalRotation = true;

        [Tooltip("Minimum vertical angle (looking down)")]
        [SerializeField][Range(-89f, 89f)] private float minVerticalAngle = -70f;

        [Tooltip("Maximum vertical angle (looking up/isometric)")]
        [SerializeField][Range(-89f, 89f)] private float maxVerticalAngle = 70f;

        private Mouse mouse;

        private void Start()
        {
            mouse = Mouse.current;
            if (inputAxisController == null) inputAxisController = GetComponent<CinemachineInputAxisController>();
            if (orbitalFollow == null) orbitalFollow = GetComponent<CinemachineOrbitalFollow>();

            ApplyLimits();
        }

        private void Update()
        {
            if (inputAxisController == null || mouse == null) return;

            bool isButtonHeld = mouseButton switch
            {
                0 => mouse.leftButton.isPressed,
                1 => mouse.rightButton.isPressed,
                2 => mouse.middleButton.isPressed,
                _ => false
            };

            // --- DEBUG SECTION ---
            if (isButtonHeld)
            {
                Vector2 mouseDelta = mouse.delta.ReadValue();
                float currentVertValue = orbitalFollow != null ? orbitalFollow.VerticalAxis.Value : 0;

                Debug.Log($"<color=cyan><b>[Camera Debug]</b></color> " +
                          $"Hold: YES | " +
                          $"Mouse Y Delta: {mouseDelta.y:F2} | " +
                          $"Camera Vert Value: {currentVertValue:F2}");
            }
            // ---------------------

            if (isButtonHeld && !inputAxisController.enabled)
            {
                inputAxisController.enabled = true;
                if (hideCursorWhileRotating) Cursor.visible = false;
            }
            else if (!isButtonHeld && inputAxisController.enabled)
            {
                inputAxisController.enabled = false;
                if (hideCursorWhileRotating) Cursor.visible = true;
            }
        }

        private void LateUpdate()
        {
            ApplyLimits();
        }

        private void ApplyLimits()
        {
            if (orbitalFollow == null) return;

            // Horizontal
            var horiz = orbitalFollow.HorizontalAxis;
            if (allowHorizontalRotation)
            {
                horiz.Range = new Vector2(-180, 180);
                horiz.Wrap = true;
            }
            else
            {
                horiz.Range = new Vector2(horiz.Value, horiz.Value);
                horiz.Wrap = false;
            }
            orbitalFollow.HorizontalAxis = horiz;

            // Vertical
            var vert = orbitalFollow.VerticalAxis;
            if (allowVerticalRotation)
            {
                vert.Range = new Vector2(minVerticalAngle, maxVerticalAngle);
                vert.Value = Mathf.Clamp(vert.Value, minVerticalAngle, maxVerticalAngle);
            }
            else
            {
                vert.Range = new Vector2(vert.Value, vert.Value);
            }
            orbitalFollow.VerticalAxis = vert;
        }
    }
}