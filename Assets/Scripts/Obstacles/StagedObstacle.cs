using System.Collections.Generic;
using Interactables;
using JetBrains.Annotations;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class StagedObstacle : MonoBehaviour, IRecordable
{
    private Rigidbody rb;
    public int currentStage = 0;
    private Vector3 targetPosition;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool movingForward = true;
    [SerializeField] private float errorMargin = 0.1f;
    private bool isMoving = false;
    [SerializeField] private bool canMoveWhileMoving = true;
    [SerializeField] private List<UnityEvent> preMoveEvents;
    [SerializeField] private List<UnityEvent> events;
    [SerializeField] private bool drawGizmos = true;

    [Foldout("Snapshot")][SerializeField] private int snapshotStage = 0;

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }

    // Debug
    private BoxCollider boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, transform.position);
        }

        if (preMoveEvents.Count > lineRenderer.positionCount)
        {
            preMoveEvents.RemoveRange(lineRenderer.positionCount, preMoveEvents.Count - lineRenderer.positionCount);
        }
        if (events.Count > lineRenderer.positionCount)
        {
            events.RemoveRange(lineRenderer.positionCount, events.Count - lineRenderer.positionCount);
        }

        EventsSizeCheck:
        if (preMoveEvents.Count < lineRenderer.positionCount)
        {
            preMoveEvents.Add(null);
            goto EventsSizeCheck;
        }
        if (events.Count < lineRenderer.positionCount)
        {
            events.Add(null);
            goto EventsSizeCheck;
        }
    }

    public virtual void FixedUpdate()
    {
        isMoving = rb.linearVelocity.magnitude > 0;

        if (Vector3.Distance(transform.position, targetPosition) <= errorMargin)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = targetPosition; // Snap to target position
            events[currentStage]?.Invoke();
        }
    }

    [Button]
    public virtual void GoToNextStage()
    {
        if (isMoving && !canMoveWhileMoving)
        {
            return;
        }

        currentStage += movingForward ? 1 : -1;
        if (currentStage >= lineRenderer.positionCount || currentStage < 0)
        {
            movingForward = !movingForward;
            currentStage = math.clamp(currentStage, 0, lineRenderer.positionCount - 1);
            currentStage += movingForward ? 1 : -1;
        }

        Debug.Log("Going to next stage: " + currentStage);
        GoToTarget();
    }

    [Button][UsedImplicitly]
    public virtual void GoToPreviousStage()
    {
        if (isMoving && !canMoveWhileMoving)
        {
            return;
        }

        currentStage += movingForward ? -1 : 1;
        if (currentStage >= lineRenderer.positionCount || currentStage < 0)
        {
            movingForward = !movingForward;
            currentStage = math.clamp(currentStage, 0, lineRenderer.positionCount - 1);
            currentStage += movingForward ? -1 : 1;
        }

        Debug.Log("Going to next stage: " + currentStage);
        GoToTarget();
    }

    public virtual void GoToStage(int stage)
    {
        if (isMoving && !canMoveWhileMoving)
        {
            return;
        }

        currentStage = stage;
        GoToTarget();
    }

    private void GoToTarget()
    {
        targetPosition = lineRenderer.GetPosition(currentStage);
        rb.linearVelocity = Vector3.zero;
        rb.AddForce((targetPosition - transform.position).normalized * moveSpeed, ForceMode.VelocityChange);

        preMoveEvents[currentStage]?.Invoke();
    }

    [Button][UsedImplicitly]
    public void SetUpLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError($"<b><color=orange>[StagedObstacle][Set Up]</color></b> {gameObject.name} tried to set up line renderer, but no line renderer found");
            return;
        }
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }

        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        if (lineRenderer != null)
        {
            for (int i = 0; i < lineRenderer.positionCount - 1; i++)
            {
                Gizmos.color = Color.rebeccaPurple;
                Gizmos.DrawLine(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
            }
            if (lineRenderer.loop)
            {
                Gizmos.DrawLine(lineRenderer.GetPosition(lineRenderer.positionCount - 1), lineRenderer.GetPosition(0));
            }

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                if (i == currentStage)
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawWireCube(
                    lineRenderer.GetPosition(i) + boxCollider.center,
                    new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z));
                Gizmos.color = Color.rebeccaPurple;
            }
        }
    }
    public void TakeSnapshot()
    {
        snapshotStage = currentStage;
    }
    public void LoadSnapshot()
    {
        targetPosition = lineRenderer.GetPosition(snapshotStage);
        transform.position = targetPosition;
        currentStage = snapshotStage;
    }
}