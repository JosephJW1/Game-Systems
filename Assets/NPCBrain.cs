using UnityEngine;
using UnityEngine.AI; // Required for NavMesh logic

// Ensure the component is on an object with a Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class PhysicsBasedPathfollower : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target; // The target to follow (e.g., the player)

    [Header("Movement")]
    public float moveSpeed = 5f; // Speed of the NPC
    public float rotationSpeed = 10f; // How fast to turn
    public float nextWaypointDistance = 1.0f; // How close to get to a waypoint before moving to the next one

    [Header("Pathfinding")]
    public float pathUpdateInterval = 0.5f; // How often to recalculate the path

    // Private component references
    private Rigidbody rb;

    // Path data
    private NavMeshPath path;
    private int currentPathIndex;
    private float pathUpdateTimer;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Initialize a new path object
        path = new NavMeshPath();

        // Start the timer to force an initial path calculation
        pathUpdateTimer = 0;
    }

    void Update()
    {
        // Update the path calculation timer
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0f)
        {
            UpdatePath();
            pathUpdateTimer = pathUpdateInterval;
        }
    }

    void FixedUpdate()
    {
        // Make sure we have a valid path and target
        if (target == null || path == null || path.corners.Length == 0)
        {
            // Stop moving if there's no path
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // Keep gravity
            return;
        }

        // Check if we've reached the end of the path
        if (currentPathIndex >= path.corners.Length)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        // Get the current waypoint we are moving towards
        Vector3 currentWaypoint = path.corners[currentPathIndex];

        // --- Handle Waypoint Switching ---
        // Calculate distance to the current waypoint (ignoring Y axis for simplicity)
        float distanceToWaypoint = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(currentWaypoint.x, 0, currentWaypoint.z)
        );

        // If we are close enough, target the next waypoint
        if (distanceToWaypoint < nextWaypointDistance)
        {
            currentPathIndex++;

            // Check if that was the last waypoint
            if (currentPathIndex >= path.corners.Length)
            {
                return; // We have arrived
            }
        }

        // --- Handle Movement ---
        // Get the direction to the *new* current waypoint
        Vector3 direction = (path.corners[currentPathIndex] - transform.position).normalized;

        // We only want to move on the XZ plane, so zero out the Y component
        direction.y = 0;

        // Set the velocity to move towards the waypoint
        // We preserve the existing Y velocity to allow for gravity
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);

        // --- Handle Rotation ---
        if (direction != Vector3.zero)
        {
            // Calculate the rotation needed to look at the direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly interpolate towards that rotation
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    /// Calculates a new path to the target.
    /// </summary>
    void UpdatePath()
    {
        if (target != null)
        {
            // Calculate the path. This returns true if a path is found.
            bool pathFound = NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

            if (pathFound)
            {
                // Reset to the beginning of the new path
                currentPathIndex = 0;
            }
            else
            {
                // No path found. Clear the current path.
                path.ClearCorners();
            }
        }
    }
}