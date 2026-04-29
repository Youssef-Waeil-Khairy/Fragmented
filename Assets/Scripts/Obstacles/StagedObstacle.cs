using System;
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
// ReSharper disable once CheckNamespace
public class StagedObstacle : MonoBehaviour, IRecordable
{
    private Rigidbody rb;
    public int currentStage = 0;
    private Vector3 targetPosition;
    private Vector3 moveDirection;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float duration;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private AnimationCurve speedCurve;
    private float timer;
    [SerializeField] private bool movingForward = true;
    [SerializeField] private float errorMargin = 0.1f;
    [SerializeField][ReadOnly] private bool isMoving = false;
    [SerializeField][ReadOnly] private bool isMovingToTarget = false;
    [SerializeField] private bool canMoveWhileMoving = true;
    [SerializeField] private List<float> distancesToTarget = new List<float>();
    [Foldout("Events")][SerializeField] private List<UnityEvent> preMoveEvents;
    [Foldout("Events")][SerializeField] private List<UnityEvent> events;
    [SerializeField] private bool drawGizmos = true;

    [Foldout("Snapshot")][SerializeField] private int snapshotStage = 0;
    [Foldout("Snapshot")][SerializeField] private int checkpointSnapshotStage = 0;
    
    [SerializeField][ReadOnly] private bool hasBeenInitialized = false;

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
        StartUp();
    }
    
    [Button]
    private void StartUp()
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

        timer = 0f;
    }

    public virtual void FixedUpdate()
    {
        isMoving = rb.linearVelocity.magnitude > 0;

        if (!isMovingToTarget) return;

        if (Vector3.Distance(transform.position, targetPosition) <= errorMargin || HasMovedAwayFromTarget())
        {
            DestinationReached:
            isMovingToTarget = false;
            rb.MovePosition(targetPosition);
            events[currentStage]?.Invoke();
            Debug.Log($"{gameObject.name} at target stage");
        }
        else
        {
            timer += Time.fixedDeltaTime;
            float time = timer / duration;
            float curveValue = speedCurve.Evaluate(time);
            float speed = math.lerp(0f, maxSpeed, curveValue);
            rb.MovePosition(transform.position + moveDirection * (speed * Time.fixedDeltaTime));
        }
        
        
    }

    private bool HasMovedAwayFromTarget()
    {
        if (distancesToTarget.Count > 1)
        {
            if (distancesToTarget[distancesToTarget.Count - 1] > distancesToTarget[distancesToTarget.Count - 2])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
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
        timer = 0f;
        isMovingToTarget = true;

        moveDirection = (targetPosition - transform.position).normalized;

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

    [Button]
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null)
        {
            Debug.LogError($"<b><color=orange>[StagedObstacle]</color></b> {gameObject.name} tried to update a line renderer, but no line renderer set up yet\nPlease run 'Set Up LineRenderer' first");
        }
        float dif = lineRenderer.GetPosition(1).y - transform.position.y;
        Debug.Log(dif);
        
        lineRenderer.SetPosition(0, transform.position);
        
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y + dif, transform.position.z));
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
            // Draw path
            for (int i = 0; i < lineRenderer.positionCount - 1; i++)
            {
                Gizmos.color = Color.rebeccaPurple;
                Gizmos.DrawLine(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
            }

            if (lineRenderer.loop)
            {
                Gizmos.DrawLine(
                    lineRenderer.GetPosition(lineRenderer.positionCount - 1),
                    lineRenderer.GetPosition(0));
            }

            // ChatGPT fix start
            // this fix from ChatGPT is to correctly preview the obstacle's positions along the line renderer
            // Save original matrix
            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Apply full transform (position, rotation, scale)
            Gizmos.matrix = transform.localToWorldMatrix;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                if (i == currentStage)
                {
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    Gizmos.color = Color.rebeccaPurple;
                }

                // Convert lineRenderer position into local space of this object
                Vector3 localPos = transform.InverseTransformPoint(lineRenderer.GetPosition(i));

                // Draw collider preview
                Gizmos.DrawWireCube(
                    localPos + boxCollider.center,
                    boxCollider.size
                    );
            }

            // Restore matrix
            Gizmos.matrix = oldMatrix;
            
            // ChatGPT fix end
        }
    }
    public void TakeSnapshot()
    {
        snapshotStage = currentStage;
    }
    public void TakeCheckpointSnapshot()
    {
        checkpointSnapshotStage = currentStage;
    }
    public void LoadSnapshot()
    {
        targetPosition = lineRenderer.GetPosition(snapshotStage);
        transform.position = targetPosition;
        currentStage = snapshotStage;
    }
    public void LoadCheckpointSnapshot()
    {
        targetPosition = lineRenderer.GetPosition(checkpointSnapshotStage);
        transform.position = targetPosition;
        currentStage = checkpointSnapshotStage;
    }
}