using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Switches cameras based on which quadrant of the play area the player is in.
/// 
/// HOW TO SET UP:
///   1. Set "Area Center" to the middle of your play area (world XZ position).
///   2. Assign Camera A = northeast, B = northwest, C = southwest, D = southeast.
///   3. That's it. The dividing lines run through Area Center on X and Z axes.
/// 
/// QUADRANT LAYOUT (viewed from above):
/// 
///        -Z
///    B   |   A
///   -----+-----  (Area Center)
///    C   |   D
///        +Z
///     -X     +X
///
/// In the Scene view you will see:
///   - A cross showing the dividing lines through the center
///   - Four coloured quadrant labels
///   - The active quadrant highlighted green
///   - Player position marked in white
/// </summary>
public class IsoCameraManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Camera References — assign in order: A (NE), B (NW), C (SW), D (SE)")]
    [SerializeField] private IsoCameraController cameraA; // +X +Z (northeast)
    [SerializeField] private IsoCameraController cameraB; // -X +Z (northwest)
    [SerializeField] private IsoCameraController cameraC; // -X -Z (southwest)
    [SerializeField] private IsoCameraController cameraD; // +X -Z (southeast)

    [Header("Play Area")]
    [Tooltip("World XZ position of the center of your play area. " +
             "The quadrant dividing lines pass through this point. " +
             "Set this to the middle of your building/level.")]
    [SerializeField] private Vector2 areaCenter = Vector2.zero;

    [Tooltip("Visual only — how large to draw the quadrant gizmo in the Scene view.")]
    [SerializeField] private float gizmoSize = 30f;

    // -----------------------------------------------------------------------

    private IsoCameraController activeCamera;
    private IsoCameraController[] allCameras;

    // -----------------------------------------------------------------------

    private IEnumerator Start()
    {
        yield return null; // let all Awake() finish

        allCameras = new[] { cameraA, cameraB, cameraC, cameraD };

        ValidateSetup();
        SwitchToCamera(GetCameraForPosition(player.position));
    }

    private void LateUpdate()
    {
        if (player == null) return;

        IsoCameraController best = GetCameraForPosition(player.position);
        if (best != activeCamera)
            SwitchToCamera(best);
    }

    // -----------------------------------------------------------------------
    // Core quadrant logic
    // -----------------------------------------------------------------------

    /// <summary>
    /// Determines which camera should be active based purely on which side of
    /// the center lines the player is on. Simple, deterministic, no flickering.
    /// </summary>
    private IsoCameraController GetCameraForPosition(Vector3 worldPos)
    {
        bool isEast = worldPos.x >= areaCenter.x;   // +X side
        bool isNorth = worldPos.z >= areaCenter.y;   // +Z side (Unity Z = forward/north)

        // Quadrant map:
        //   East  + North = A (northeast)
        //   West  + North = B (northwest)
        //   West  + South = C (southwest)
        //   East  + South = D (southeast)

        if (isEast && isNorth) return cameraA;
        if (!isEast && isNorth) return cameraB;
        if (!isEast && !isNorth) return cameraC;
        return cameraD;
    }

    private void SwitchToCamera(IsoCameraController next)
    {
        if (next == null || next == activeCamera) return;

        // Deactivate all, then activate the chosen one
        if (allCameras != null)
            foreach (var cam in allCameras)
                if (cam != null) cam.SetActive(false);

        next.SetActive(true);
        activeCamera = next;
    }

    // -----------------------------------------------------------------------

    private void ValidateSetup()
    {
        if (player == null) Debug.LogError("[IsoCameraManager] Player not assigned.", this);
        if (cameraA == null) Debug.LogError("[IsoCameraManager] Camera A not assigned.", this);
        if (cameraB == null) Debug.LogError("[IsoCameraManager] Camera B not assigned.", this);
        if (cameraC == null) Debug.LogError("[IsoCameraManager] Camera C not assigned.", this);
        if (cameraD == null) Debug.LogError("[IsoCameraManager] Camera D not assigned.", this);
    }

    // -----------------------------------------------------------------------
    // GIZMOS
    // -----------------------------------------------------------------------

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        float cx = areaCenter.x;
        float cz = areaCenter.y;
        float s = gizmoSize;

        // Dividing cross lines
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.DrawLine(
            new Vector3(cx - s, 0, cz),
            new Vector3(cx + s, 0, cz));   // horizontal line (Z axis)
        UnityEditor.Handles.DrawLine(
            new Vector3(cx, 0, cz - s),
            new Vector3(cx, 0, cz + s));   // vertical line (X axis)

        // Center dot
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(new Vector3(cx, 0, cz), 0.5f);
        UnityEditor.Handles.Label(new Vector3(cx + 0.6f, 0, cz), "Center");

        // Determine which quadrant the player is in for highlighting
        bool inEast = player != null && player.position.x >= cx;
        bool inNorth = player != null && player.position.z >= cz;

        // Quadrant A — northeast (+X, +Z)
        bool aActive = Application.isPlaying
            ? activeCamera == cameraA
            : (inEast && inNorth);
        DrawQuadrant(
            new Vector3(cx + s * 0.5f, 0, cz + s * 0.5f),
            "A (NE)\nCamera A", aActive, Color.green, Color.yellow);

        // Quadrant B — northwest (-X, +Z)
        bool bActive = Application.isPlaying
            ? activeCamera == cameraB
            : (!inEast && inNorth);
        DrawQuadrant(
            new Vector3(cx - s * 0.5f, 0, cz + s * 0.5f),
            "B (NW)\nCamera B", bActive, Color.green, Color.cyan);

        // Quadrant C — southwest (-X, -Z)
        bool cActive = Application.isPlaying
            ? activeCamera == cameraC
            : (!inEast && !inNorth);
        DrawQuadrant(
            new Vector3(cx - s * 0.5f, 0, cz - s * 0.5f),
            "C (SW)\nCamera C", cActive, Color.green, Color.magenta);

        // Quadrant D — southeast (+X, -Z)
        bool dActive = Application.isPlaying
            ? activeCamera == cameraD
            : (inEast && !inNorth);
        DrawQuadrant(
            new Vector3(cx + s * 0.5f, 0, cz - s * 0.5f),
            "D (SE)\nCamera D", dActive, Color.green, new Color(1f, 0.5f, 0f));

        // Player marker
        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(player.position, 0.6f);
            // Line from player to center
            UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.3f);
            UnityEditor.Handles.DrawLine(player.position, new Vector3(cx, player.position.y, cz));
        }
    }

    private static void DrawQuadrant(Vector3 labelPos, string label,
        bool isActive, Color activeCol, Color inactiveCol)
    {
        UnityEditor.Handles.color = isActive ? activeCol : new Color(inactiveCol.r, inactiveCol.g, inactiveCol.b, 0.5f);
        UnityEditor.Handles.Label(labelPos, label);

        // Dot at quadrant center
        Gizmos.color = isActive ? activeCol : new Color(inactiveCol.r, inactiveCol.g, inactiveCol.b, 0.4f);
        Gizmos.DrawSphere(labelPos, isActive ? 0.8f : 0.4f);
    }
#endif
}