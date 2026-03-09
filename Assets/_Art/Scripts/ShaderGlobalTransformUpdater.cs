using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShaderGlobalTransformUpdater : MonoBehaviour
{
    [SerializeField]
    string globalPositionReference;

    [SerializeField]
    string globalScaleReference;

    [SerializeField]
    string globalRotationReference;

    [Header("Gizmo Settings")]
    public Color gizmoColor = new Color(0, 1, 1, 0.8f);
    private float radius = 0.5f;

    [Range(16, 128)]
    private int circleSegments = 64;

    private void Update()
    {
        Shader.SetGlobalVector(globalPositionReference, transform.position);
        Shader.SetGlobalVector(globalScaleReference, transform.localScale);
        Shader.SetGlobalVector(globalRotationReference, transform.eulerAngles);
    }

    private void OnDrawGizmos()
    {
        DrawSphereColliderGizmo();
        DrawBillboardCircle();
    }

    // ============================
    // 3D Sphere (Collider Style)
    // ============================
    private void DrawSphereColliderGizmo()
    {
        Gizmos.color = gizmoColor;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            transform.rotation,
            transform.lossyScale
        );

        Gizmos.DrawWireSphere(Vector3.zero, radius);

        Gizmos.matrix = oldMatrix;
    }

    // ============================
    // Billboard Circle (Outline)
    // ============================
    private void DrawBillboardCircle()
    {
        if (Camera.current == null)
            return;

        Camera cam = Camera.current;

        Gizmos.color = gizmoColor;

        Vector3 center = transform.position;

        // Use camera axes (no distortion)
        Vector3 camRight = cam.transform.right;
        Vector3 camUp = cam.transform.up;

        float scaledRadius = radius * GetMaxScale();

        Vector3 prev = Vector3.zero;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / circleSegments;

            Vector3 dir =
                camRight * Mathf.Cos(angle) +
                camUp * Mathf.Sin(angle);

            Vector3 p = center + dir * scaledRadius;

            if (i > 0)
                Gizmos.DrawLine(prev, p);

            prev = p;
        }
    }

    private float GetMaxScale()
    {
        Vector3 s = transform.lossyScale;
        return Mathf.Max(s.x, s.y, s.z);
    }
}
