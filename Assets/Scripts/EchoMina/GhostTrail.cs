using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GhostTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public float meshRefreshRate = 0.05f;
    public float meshDestroyDelay = 0.5f;
    public float movementThreshold = 0.01f;

    [Header("Visuals")]
    public Material ghostMaterial;
    public string alphaPropertyName = "_Alpha";

    [Header("Target Model")]
    [SerializeField] private GameObject targetModelParent;

    private MeshRenderer[] meshRenderers;
    private MeshFilter[] meshFilters;
    private Vector3 lastPosition;
    private Coroutine spawnCoroutine;

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
        if (targetModelParent == null) targetModelParent = this.gameObject;

        meshRenderers = targetModelParent.GetComponentsInChildren<MeshRenderer>(true);
        meshFilters = targetModelParent.GetComponentsInChildren<MeshFilter>(true);
    }

    // ✅ Runs every time the GameObject is enabled (including after SetActive(true))
    void OnEnable()
    {
        lastPosition = transform.position;
        activeGhosts.Clear();

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnGhosts());
    }

    // ✅ Runs every time the GameObject is disabled
    void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        activeGhosts.Clear();
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
                    newGhost.material.renderQueue = (int)RenderQueue.Transparent + 100;
                    newGhost.material.SetInt("_ZWrite", 0);
                    newGhost.startTime = Time.time;
                    activeGhosts.Add(newGhost);
                }
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
                    0, null, 0, null,
                    ShadowCastingMode.Off,
                    false, null,
                    LightProbeUsage.Off, null
                );
            }
        }
    }
}