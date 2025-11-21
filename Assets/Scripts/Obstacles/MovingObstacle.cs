using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private LineRenderer pathRenderer;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float errorMargin = 0.1f;
    private int currentPointIndex = 0;
    private int pointDirection = 1; // 1 for forward, -1 for backward
    private Rigidbody rb;
    private Vector3 targetPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get all needed references
        rb = GetComponent<Rigidbody>();

        // Ensure the path has at least two points
        if (pathRenderer.positionCount < 2)
        {
            Debug.LogError("Path must have at least two points.");
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the obstacle has reached the target point
        if (Vector3.Distance(transform.position, targetPoint) <= errorMargin)
        {
            // Check if we are on the last point
            if (currentPointIndex == pathRenderer.positionCount - 1)
            {
                pointDirection = -1; // Reverse direction
            }
            else if (currentPointIndex == 0)
            {
                pointDirection = 1; // Forward direction
            }
            // Move to the next point in the path
            currentPointIndex += pointDirection;
            MoveToPoint(currentPointIndex);
        }
    }

    public void SetTargetPoint(Vector3 point)
    {
        targetPoint = point;
    }
    public void SetTargetPoint(int index)
    {
        if (index >= 0 && index < pathRenderer.positionCount)
        {
            currentPointIndex = index;
            targetPoint = pathRenderer.GetPosition(currentPointIndex);
        }
        else
        {
            Debug.LogError("Index out of bounds for path points.");
        }
    }

    public void MoveToPoint()
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }
    public void MoveToPoint(Vector3 point)
    {
        SetTargetPoint(point);
        MoveToPoint();
    }
    public void MoveToPoint(int index)
    {
        SetTargetPoint(index);
        MoveToPoint();
    }

    void OnDrawGizmosSelected()
    {
        // Draw the path in the editor for visualization
        if (pathRenderer != null && pathRenderer.positionCount > 1)
        {
            Vector3 outline = GetComponent<BoxCollider>().size;
            Gizmos.color = Color.green;
            for (int i = 0; i < pathRenderer.positionCount - 1; i++)
            {
                Gizmos.DrawLine(pathRenderer.GetPosition(i), pathRenderer.GetPosition(i + 1));
                Gizmos.DrawWireCube(pathRenderer.GetPosition(i), outline);
            }
            Gizmos.DrawWireCube(pathRenderer.GetPosition(pathRenderer.positionCount - 1), outline);
        }
    }
}
