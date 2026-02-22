using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Simple fixed isometric camera. No rotation controls.
/// Sits at its corner, tracks player Y, that's it.
/// Activated/deactivated by IsoCameraManager based on which quadrant the player is in.
/// </summary>
[RequireComponent(typeof(CinemachineCamera))]
public class IsoCameraController : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Isometric Rotation (fixed)")]
    [SerializeField] private float pitchAngle = 35.264f;
    [SerializeField] private float yawAngle = 225f;

    [Header("Position")]
    [Tooltip("Fixed XZ world position of this camera corner.")]
    [SerializeField] private Vector2 cornerXZ = new Vector2(40f, 40f);
    [Tooltip("Height above the player's Y position.")]
    [SerializeField] private float verticalOffset = 25f;

    // -----------------------------------------------------------------------

    private CinemachineCamera vcam;
    private bool isActive = false;

    private const int PRIORITY_ACTIVE = 20;
    private const int PRIORITY_INACTIVE = 0;

    // -----------------------------------------------------------------------

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        if (vcam == null)
        {
            Debug.LogError($"[IsoCameraController] '{gameObject.name}' needs a CinemachineCamera.", this);
            return;
        }
        vcam.Priority = PRIORITY_INACTIVE;

        if (player == null)
            Debug.LogError($"[IsoCameraController] '{gameObject.name}' has no Player assigned.", this);
    }

    private void LateUpdate()
    {
        if (!isActive || player == null) return;
        ApplyTransform();
    }

    // -----------------------------------------------------------------------

    public void SetActive(bool active)
    {
        isActive = active;
        vcam.Priority = active ? PRIORITY_ACTIVE : PRIORITY_INACTIVE;
        if (active) ApplyTransform();
    }

    private void ApplyTransform()
    {
        transform.SetPositionAndRotation(
            new Vector3(cornerXZ.x, player.position.y + verticalOffset, cornerXZ.y),
            Quaternion.Euler(pitchAngle, yawAngle, 0f)
        );
    }

    // -----------------------------------------------------------------------

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 pos = new Vector3(cornerXZ.x, transform.position.y, cornerXZ.y);
        Gizmos.color = isActive ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(pos, 1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(pos, Quaternion.Euler(pitchAngle, yawAngle, 0f) * Vector3.forward * 5f);
    }
#endif
}