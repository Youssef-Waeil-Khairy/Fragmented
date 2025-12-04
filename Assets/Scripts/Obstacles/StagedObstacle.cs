using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class StagedObstacle : MonoBehaviour
{
    private Rigidbody rb;
    public int currentStage = 0;
    private Vector3 targetPosition;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int stageDirection = 1;
    [SerializeField] private float errorMargin = 0.1f;
    private bool isMoving = false;
    [SerializeField] private UnityEvent events;

    public bool IsMoving => isMoving;

    // Debug
    private BoxCollider boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, transform.position);
        }
    }

    private void FixedUpdate()
    {
        isMoving = rb.linearVelocity.magnitude > 0;

        if (Vector3.Distance(transform.position, targetPosition) <= errorMargin)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = targetPosition; // Snap to target position
            events?.Invoke();
        }
    }

    public void GoToNextStage()
    {
        if (isMoving)
        {
            return;
        }
        if (lineRenderer.loop)
        {
            currentStage = (currentStage++) % lineRenderer.positionCount; // BUG: This needs to be looked at, seems broken
        }
        else
        {
            if (currentStage == lineRenderer.positionCount - 1)
            {
                stageDirection = -1;
            }
            else if (currentStage == 0)
            {
                stageDirection = 1;
            }
            currentStage += stageDirection;
        }
        targetPosition = lineRenderer.GetPosition(currentStage);
        rb.linearVelocity = (targetPosition - transform.position).normalized * moveSpeed;
        events?.Invoke();
    }

    private void OnDrawGizmos()
    {
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
                    lineRenderer.GetPosition(i),
                    new Vector3(boxCollider.size.x * transform.localScale.x,
                    boxCollider.size.y  * transform.localScale.y,
                    boxCollider.size.z  * transform.localScale.z));
                Gizmos.color = Color.rebeccaPurple;
            }
        }
    }
}