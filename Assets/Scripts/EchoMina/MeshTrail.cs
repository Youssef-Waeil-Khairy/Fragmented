using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EchoMina.Original;

public class MeshTrail : MonoBehaviour
{
    [Header("Mesh Related Stuff")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyedDelay = 3f;

    [Header("Shader Related Stuff")]
    public Material mat;
    public string sahderVarRef;
    public float sahderVarRate = 0.1f;
    public float sahderVarRefreshRate = 0.05f;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private Coroutine trailCoroutine;

    // Tracks every spawned frame so we can nuke them instantly on stop
    private readonly List<GameObject> activeFrames = new List<GameObject>();

    private void OnEnable()
    {
        if (OriginalEchoMina.Instance != null)
        {
            OriginalEchoMina.Instance.StartRecording += StartTrail;
            OriginalEchoMina.Instance.StartPlayback += StartTrail;
            OriginalEchoMina.Instance.StopRecording += StopTrail;
            OriginalEchoMina.Instance.StopPlayback += StopTrail;
        }
    }

    private void OnDisable()
    {
        if (OriginalEchoMina.Instance != null)
        {
            OriginalEchoMina.Instance.StartRecording -= StartTrail;
            OriginalEchoMina.Instance.StartPlayback -= StartTrail;
            OriginalEchoMina.Instance.StopRecording -= StopTrail;
            OriginalEchoMina.Instance.StopPlayback -= StopTrail;
        }
        StopTrail();
    }

    public void StartTrail()
    {
        if (!isTrailActive)
        {
            isTrailActive = true;
            trailCoroutine = StartCoroutine(ActivateTrail());
        }
    }

    public void StopTrail()
    {
        isTrailActive = false;

        // Stop ALL coroutines on this component — kills ActivateTrail
        // AND every AnimateMaterial that was still running
        StopAllCoroutines();
        trailCoroutine = null;

        // Immediately destroy every frame that's still in the scene
        ClearAllFrames();
    }

    private void ClearAllFrames()
    {
        for (int i = 0; i < activeFrames.Count; i++)
        {
            if (activeFrames[i] != null)
                Destroy(activeFrames[i]);
        }
        activeFrames.Clear();
    }

    IEnumerator ActivateTrail()
    {
        while (isTrailActive && gameObject.activeInHierarchy)
        {
            if (skinnedMeshRenderers == null || skinnedMeshRenderers.Length == 0)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                if (!skinnedMeshRenderers[i].gameObject.activeInHierarchy) continue;

                GameObject gObj = new GameObject("TrailFrame");
                gObj.transform.SetPositionAndRotation(
                    skinnedMeshRenderers[i].transform.position,
                    skinnedMeshRenderers[i].transform.rotation
                );

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;

                Material matInstance = Instantiate(mat);
                mr.material = matInstance;

                activeFrames.Add(gObj);

                // Pass the index so we can remove it from the list when it dies naturally
                StartCoroutine(AnimateMaterial(gObj, matInstance, i));
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }

    IEnumerator AnimateMaterial(GameObject frame, Material material, int listIndex)
    {
        float valueToAnimate = material.GetFloat(sahderVarRef);

        while (valueToAnimate > 0f)
        {
            valueToAnimate -= sahderVarRate;
            material.SetFloat(sahderVarRef, Mathf.Max(valueToAnimate, 0f));
            yield return new WaitForSeconds(sahderVarRefreshRate);
        }

        // Natural fade finished — clean up
        if (frame != null)
        {
            activeFrames.Remove(frame);
            Destroy(frame);
        }
    }
}