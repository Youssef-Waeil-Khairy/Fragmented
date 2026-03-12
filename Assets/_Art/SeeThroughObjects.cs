using System.Collections.Generic;
using UnityEngine;

public class SeeThroughObjects : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private List<Transform> additionalTargets = new List<Transform>();

    [Header("Transparency Settings")]
    [SerializeField][Range(0f, 1f)] private float hiddenAlpha = 0.15f;
    [SerializeField] private float fadeSpeed = 8f;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask obstacleLayers;

    // Tracks all objects currently faded and their original materials
    private Dictionary<Renderer, OriginalMaterialData> fadedObjects
        = new Dictionary<Renderer, OriginalMaterialData>();

    private class OriginalMaterialData
    {
        public Material[] materials;
        public float[] originalAlphas;
        public RenderingMode[] originalModes;
    }

    private enum RenderingMode { Opaque, Cutout, Fade, Transparent }

    void Update()
    {
        // Collect all objects blocking any target this frame
        HashSet<Renderer> blockingThisFrame = new HashSet<Renderer>();

        // Always check the main player
        if (player != null)
            CheckTarget(player, blockingThisFrame);

        // Check any additional targets (eg Echo Mina)
        foreach (Transform target in additionalTargets)
        {
            if (target != null)
                CheckTarget(target, blockingThisFrame);
        }

        // Fade out blocking objects
        foreach (Renderer rend in blockingThisFrame)
        {
            if (!fadedObjects.ContainsKey(rend))
                StoreAndFade(rend);
            else
                FadeToAlpha(rend, hiddenAlpha);
        }

        // Restore objects no longer blocking
        List<Renderer> toRestore = new List<Renderer>();
        foreach (var kvp in fadedObjects)
        {
            if (!blockingThisFrame.Contains(kvp.Key))
                toRestore.Add(kvp.Key);
        }
        foreach (Renderer rend in toRestore)
            RestoreObject(rend);
    }

    // ─────────────────────────────────────────
    // Cast ray from camera to target, collect blockers
    // ─────────────────────────────────────────
    private void CheckTarget(Transform target, HashSet<Renderer> blockers)
    {
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position,
            direction.normalized,
            distance,
            obstacleLayers
        );

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
                blockers.Add(rend);
        }
    }

    // ─────────────────────────────────────────
    // Store original material data and start fading
    // ─────────────────────────────────────────
    private void StoreAndFade(Renderer rend)
    {
        Material[] mats = rend.materials;
        float[] alphas = new float[mats.Length];
        RenderingMode[] modes = new RenderingMode[mats.Length];

        for (int i = 0; i < mats.Length; i++)
        {
            alphas[i] = mats[i].color.a;
            modes[i] = GetRenderingMode(mats[i]);
            SetRenderingMode(mats[i], RenderingMode.Fade);
        }

        fadedObjects[rend] = new OriginalMaterialData
        {
            materials = mats,
            originalAlphas = alphas,
            originalModes = modes
        };
    }

    // ─────────────────────────────────────────
    // Smoothly fade a renderer to target alpha
    // ─────────────────────────────────────────
    private void FadeToAlpha(Renderer rend, float targetAlpha)
    {
        foreach (Material mat in rend.materials)
        {
            Color c = mat.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            mat.color = c;
        }
    }

    // ─────────────────────────────────────────
    // Restore a renderer to its original state
    // ─────────────────────────────────────────
    private void RestoreObject(Renderer rend)
    {
        if (!fadedObjects.ContainsKey(rend)) return;

        OriginalMaterialData data = fadedObjects[rend];
        Material[] mats = rend.materials;

        for (int i = 0; i < mats.Length; i++)
        {
            Color c = mats[i].color;
            c.a = Mathf.Lerp(c.a, data.originalAlphas[i], Time.deltaTime * fadeSpeed);
            mats[i].color = c;

            // Once close to original alpha, snap back and restore render mode
            if (Mathf.Abs(c.a - data.originalAlphas[i]) < 0.01f)
            {
                c.a = data.originalAlphas[i];
                mats[i].color = c;
                SetRenderingMode(mats[i], data.originalModes[i]);
            }
        }

        // Only remove from dictionary once fully restored
        if (Mathf.Abs(mats[0].color.a - data.originalAlphas[0]) < 0.01f)
            fadedObjects.Remove(rend);
    }

    // ─────────────────────────────────────────
    // Unity Standard Shader rendering mode helpers
    // ─────────────────────────────────────────
    private RenderingMode GetRenderingMode(Material mat)
    {
        return (RenderingMode)(int)mat.GetFloat("_Mode");
    }

    private void SetRenderingMode(Material mat, RenderingMode mode)
    {
        mat.SetFloat("_Mode", (float)mode);

        switch (mode)
        {
            case RenderingMode.Opaque:
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = -1;
                break;

            case RenderingMode.Fade:
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                break;

            case RenderingMode.Transparent:
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                break;
        }
    }
}