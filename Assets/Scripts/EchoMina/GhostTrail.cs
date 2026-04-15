using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public float meshRefreshRate = 0.05f;   // How often a ghost spawns
    public float meshDestroyDelay = 0.5f;    // How long each ghost stays visible

    [Header("Visuals")]
    public Material ghostMaterial;         // Assign your GhostMaterial here
    public string alphaPropertyName = "_Alpha"; // Must match shader reference

    [Header("Target Model")]
    [SerializeField] private GameObject targetModelParent; // Drag the parent GameObject of your Mina Model here

    private MeshRenderer[] meshRenderers;
    private MeshFilter[] meshFilters;

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
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (meshRenderers[i] == null || !meshRenderers[i].enabled || meshFilters[i] == null || meshFilters[i].sharedMesh == null) continue;

                GhostData newGhost = new GhostData();
                newGhost.mesh = meshFilters[i].sharedMesh;
                newGhost.matrix = meshRenderers[i].transform.localToWorldMatrix;
                newGhost.material = new Material(ghostMaterial);

                // Set render queue to draw behind transparent objects but after opaque ones
                newGhost.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent - 1; // Draw just before transparent objects

                newGhost.startTime = Time.time;

                activeGhosts.Add(newGhost);
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
                Destroy(activeGhosts[i].material);
                activeGhosts.RemoveAt(i);
                continue;
            }

            // Update Alpha
            float currentAlpha = Mathf.Lerp(1f, 0f, t);
            activeGhosts[i].material.SetFloat(alphaPropertyName, currentAlpha);

            // Draw the mesh directly in the world, explicitly disabling shadows
            Graphics.DrawMesh(activeGhosts[i].mesh, activeGhosts[i].matrix, activeGhosts[i].material, 0, null, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
        }
    }
}