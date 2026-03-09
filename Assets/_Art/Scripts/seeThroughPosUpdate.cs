using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class seeThroughPosUpdater : MonoBehaviour
{

    // =============================
    // CUTOUT GLOBAL REFERENCES
    // =============================
    [Header("Cutout Global References")]
    [SerializeField] string globalCutoutPosReference = "_CutoutPos";
    [SerializeField] string globalCutoutSizeReference = "_CutoutSize";
    [SerializeField] string globalFalloffReference = "_FalloffSize";

    [SerializeField] private Transform targetObject;
    [SerializeField] private float cutoutSize = 0.1f;
    [SerializeField] private float falloffSize = 0.05f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        UpdateGlobalCutout();
    }

    // =============================
    // GLOBAL CUTOUT UPDATE
    // =============================
    private void UpdateGlobalCutout()
    {

        // World → Viewport
        Vector3 viewport = mainCamera.WorldToViewportPoint(targetObject.position);
        Vector2 cutoutPos = new Vector2(viewport.x, viewport.y);
        cutoutPos.y /= (Screen.width / Screen.height);

        Shader.SetGlobalVector(globalCutoutPosReference, cutoutPos);
        Shader.SetGlobalFloat(globalCutoutSizeReference, cutoutSize);
        Shader.SetGlobalFloat(globalFalloffReference, falloffSize);
    }
}
