using System.Collections;
using UnityEngine;
using EchoMina.Original;

public class MeshTrail : MonoBehaviour
{
    public float meshRefreshRate = 0.1f;
    private bool isTrailActive;
    [SerializeField] private bool isSubscribed;
    public GameObject parentObjectWithRenderers; // Assign the parent GameObject in the Inspector
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    private void Awake()
    {
        Debug.Log($"[MeshTrail] Awake fired on: {gameObject.name}. Script enabled: {enabled}. GameObject active: {gameObject.activeInHierarchy}");

        OriginalEchoMina.OnInstanceReady += Subscribe;

        if (OriginalEchoMina.Instance != null)
        {
            Debug.Log("[MeshTrail] Instance already ready on Awake.");
            Subscribe();
        }
    }

    private void OnEnable()
    {
        Debug.Log($"[MeshTrail] OnEnable fired on: {gameObject.name}. Script enabled: {enabled}. GameObject active: {gameObject.activeInHierarchy}");
        // Attempt to subscribe if not already subscribed and EchoMina is ready
        if (!isSubscribed && OriginalEchoMina.Instance != null)
        {
            Subscribe();
        }
    }

    private void Start()
    {
        Debug.Log($"[MeshTrail] Start fired. isSubscribed={isSubscribed}. Script enabled: {enabled}. GameObject active: {gameObject.activeInHierarchy}");
    }

    private void Subscribe()
    {
        if (isSubscribed)
        {
            Debug.Log("[MeshTrail] Already subscribed, returning.");
            return;
        }

        Debug.Log("[MeshTrail] Attempting to subscribe to EchoMina events.");
        OriginalEchoMina.Instance.StartPlayback += OnEchoStartPlayback;
        OriginalEchoMina.Instance.StopPlayback += OnEchoStopPlayback;
        isSubscribed = true;
        Debug.Log("[MeshTrail] Successfully subscribed to EchoMina events.");
    }

    private void OnDestroy()
    {
        Debug.Log("[MeshTrail] OnDestroy fired. Unsubscribing from EchoMina events.");
        OriginalEchoMina.OnInstanceReady -= Subscribe;
        if (isSubscribed && OriginalEchoMina.Instance != null)
        {
            OriginalEchoMina.Instance.StartPlayback -= OnEchoStartPlayback;
            OriginalEchoMina.Instance.StopPlayback -= OnEchoStopPlayback;
            isSubscribed = false;
        }
    }

    private void OnDisable()
    {
        Debug.Log("[MeshTrail] OnDisable fired. Unsubscribing from EchoMina events.");
        if (isSubscribed && OriginalEchoMina.Instance != null)
        {
            OriginalEchoMina.Instance.StartPlayback -= OnEchoStartPlayback;
            OriginalEchoMina.Instance.StopPlayback -= OnEchoStopPlayback;
            isSubscribed = false;
        }
    }

    private void OnEchoStartPlayback()
    {
        Debug.Log("[MeshTrail] OnEchoStartPlayback triggered. Playback started!");

        // Find renderers from the assigned parent object
        if (parentObjectWithRenderers != null)
        {
            skinnedMeshRenderers = parentObjectWithRenderers.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            Debug.Log($"[MeshTrail] Found {skinnedMeshRenderers.Length} renderers on parentObjectWithRenderers.");
        }
        else
        {
            Debug.LogError("[MeshTrail] parentObjectWithRenderers is not assigned. Please assign a GameObject in the Inspector.");
            return; // Stop execution if no parent object is assigned
        }

        if (isTrailActive) return;
        isTrailActive = true;
        StartCoroutine(ActivateTrail());
    }

    private void OnEchoStopPlayback()
    {
        Debug.Log("[MeshTrail] OnEchoStopPlayback triggered. Playback stopped.");
        isTrailActive = false;
    }

    IEnumerator ActivateTrail()
    {
        Debug.Log("[MeshTrail] Trail coroutine running.");
        int count = 0;

        while (isTrailActive)
        {
            if (skinnedMeshRenderers == null || skinnedMeshRenderers.Length == 0)
            {
                Debug.LogError("[MeshTrail] No renderers! Stopping trail.");
                yield break;
            }

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                Mesh bakedMesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(bakedMesh);

                GameObject gObj = new GameObject($"TrailMesh_{count}");
                gObj.transform.position = skinnedMeshRenderers[i].transform.position;
                gObj.transform.rotation = skinnedMeshRenderers[i].transform.rotation;
                gObj.transform.localScale = skinnedMeshRenderers[i].transform.lossyScale;

                MeshFilter mf = gObj.AddComponent<MeshFilter>();
                mf.mesh = bakedMesh;

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                mr.materials = skinnedMeshRenderers[i].materials;

                Destroy(gObj, meshRefreshRate * 10f);
                count++;
            }

            Debug.Log($"[MeshTrail] Snapshot {count / skinnedMeshRenderers.Length}");
            yield return new WaitForSeconds(meshRefreshRate);
        }

        Debug.Log("[MeshTrail] Trail coroutine ended.");
    }
}