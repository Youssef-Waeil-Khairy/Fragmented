using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlateController : MonoBehaviour
{
    [Header("Stairs Animator")]
    public Animator stairsAnimator;
    public string parameterName = "IsPressed";

    [Header("Player Tag")]
    public string playerTag = "Player"; // Only reacts to player

    private int objectsOnPlate = 0;
    private int paramHash;

    void Awake()
    {
        if (stairsAnimator == null)
            Debug.LogWarning("[PressurePlate] No stairs Animator assigned!", this);

        paramHash = Animator.StringToHash(parameterName);

        // Ensure the collider is a trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning("[PressurePlate] Collider was not a trigger. Fixed automatically.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return; // ignore everything else

        objectsOnPlate++;
        Debug.Log($"[PressurePlate] Player entered → count = {objectsOnPlate}");

        if (objectsOnPlate == 1)
        {
            stairsAnimator.SetBool(paramHash, true);
            Debug.Log("[PressurePlate] Set IsPressed = true on stairs Animator");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        objectsOnPlate = Mathf.Max(0, objectsOnPlate - 1);
        Debug.Log($"[PressurePlate] Player exited → count = {objectsOnPlate}");

        if (objectsOnPlate == 0)
        {
            stairsAnimator.SetBool(paramHash, false);
            Debug.Log("[PressurePlate] Set IsPressed = false on stairs Animator");
        }
    }

    // optional: check in Inspector
    public int GetObjectsOnPlateCount() => objectsOnPlate;
}
