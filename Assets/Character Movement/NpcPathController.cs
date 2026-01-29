using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BaseMover))]
public class NpcPathController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Pathfinding")]
    public float pathUpdateRate = 0.5f;
    public float waypointThreshold = 1.0f;
    public float stopDistance = 1.5f;

    private BaseMover mover;
    private NavMeshPath path;
    private int currentWaypointIndex;

    void Start()
    {
        mover = GetComponent<BaseMover>();
        path = new NavMeshPath();
        InvokeRepeating(nameof(CalculateNewPath), 0f, pathUpdateRate);
    }

    void CalculateNewPath()
    {
        if (target != null)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            currentWaypointIndex = 1;
        }
    }

    void Update()
    {
        if (path == null || path.status == NavMeshPathStatus.PathInvalid || currentWaypointIndex >= path.corners.Length)
        {
            mover.Move(Vector3.zero);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < stopDistance)
        {
            mover.Move(Vector3.zero);
            return;
        }

        Vector3 waypoint = path.corners[currentWaypointIndex];
        Vector3 dirToWaypoint = (waypoint - transform.position);
        dirToWaypoint.y = 0;

        mover.Move(dirToWaypoint.normalized);

        float distanceToWaypoint = dirToWaypoint.magnitude;
        if (distanceToWaypoint < waypointThreshold)
        {
            currentWaypointIndex++;
        }
    }
}