using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GhostTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public float meshRefreshRate = 0.05f;   // How often a ghost spawns
    public float meshDestroyDelay = 0.5f;    // How long each ghost stays visible
    public float movementThreshold = 0.01f; // Minimum distance moved to spawn a ghost

    [Header("Visuals")]
    public Material ghostMaterial;         // Assign your GhostMaterial here
    public string alphaPropertyName = "_Alpha"; // Must match shader reference

    [Header("Target Model")]
    [SerializeField] private GameObject targetModelParent; // Drag the parent GameObject of your Mina Model here

    private MeshRenderer[] meshRenderers;
    private MeshFilter[] meshFilters;
    private Vector3 lastPosition;

    private class GhostData
    {
        public Mesh mesh;
        public Matrix4x4 matrix;
        public Material material;
        public float startTime;
    }

    private List<GhostData> activeGhosts = new List<GhostData>();

    void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(SpawnGhosts());
    }

    IEnumerator SpawnGhosts()
    {
        yield return new WaitForSeconds(0.1f);

        if (targetModelParent == null) targetModelParent = this.gameObject;

        meshRenderers = targetModelParent.GetComponentsInChildren<MeshRenderer>(true);
        meshFilters = targetModelParent.GetComponentsInChildren<MeshFilter>(true);

        if (meshRenderers == null || meshRenderers.Length == 0)
        {
            Debug.LogError("GhostTrail: No MeshRenderers found!");
            yield break;
        }

        while (true)
        {
            // Check if the character has moved significantly since the last frame
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved > movementThreshold)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    if (meshRenderers[i] == null || !meshRenderers[i].enabled || meshFilters[i] == null || meshFilters[i].sharedMesh == null) continue;

                    GhostData newGhost = new GhostData();
                    newGhost.mesh = meshFilters[i].sharedMesh;
                    newGhost.matrix = meshRenderers[i].transform.localToWorldMatrix;
                    newGhost.material = new Material(ghostMaterial);

                    // Force settings to prevent artifacts
                    newGhost.material.renderQueue = (int)RenderQueue.Transparent + 100;
                    newGhost.material.SetInt("_ZWrite", 0);

                    newGhost.startTime = Time.time;
                    activeGhosts.Add(newGhost);
                }
                // Update lastPosition only when we actually spawn a ghost
                lastPosition = transform.position;
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }
    }

    void Update()
    {
        for (int i = activeGhosts.Count - 1; i >= 0; i--)
        {
            float elapsed = Time.time - activeGhosts[i].startTime;
            float t = elapsed / meshDestroyDelay;

            if (t >= 1.0f)
            {
                if (activeGhosts[i].material != null) Destroy(activeGhosts[i].material);
                activeGhosts.RemoveAt(i);
                continue;
            }

            float currentAlpha = Mathf.Lerp(1f, 0f, t);
            if (activeGhosts[i].material != null)
            {
                activeGhosts[i].material.SetFloat(alphaPropertyName, currentAlpha);

                Graphics.DrawMesh(
                    activeGhosts[i].mesh,
                    activeGhosts[i].matrix,
                    activeGhosts[i].material,
                    0,
                    null,
                    0,
                    null,
                    ShadowCastingMode.Off,
                    false,
                    null,
                    LightProbeUsage.Off,
                    null
                );
            }
        }
    }
}
