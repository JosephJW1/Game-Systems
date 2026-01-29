using System.Collections.Generic;
using UnityEngine;

public class TrainFollowTransform : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Drag the GameObject you want the train to chase here.")]
    public Transform targetTransform;
    public float speed = 5f;
    [Tooltip("How close the train gets to the target before stopping.")]
    public float stoppingDistance = 1.0f;

    [Header("Train Settings")]
    public GameObject carPrefab;
    public int numberOfCars = 5;
    [Tooltip("Distance between each car.")]
    public float spacing = 1.5f;

    // Internal storage
    private List<GameObject> trainCars = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();

    private void Start()
    {
        if (targetTransform == null)
        {
            Debug.LogError("Please assign a Target Transform in the Inspector!");
            return;
        }

        // Initialize history with current position to prevent snapping
        positionHistory.Add(transform.position);

        SpawnTrain();
    }

    private void Update()
    {
        if (targetTransform == null) return;

        // 1. Move the Head
        MoveHead();

        // 2. Update the Body parts
        UpdateBodyPositions();

        // 3. Cleanup history
        TrimHistory();
    }

    private void SpawnTrain()
    {
        for (int i = 0; i < numberOfCars; i++)
        {
            GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);
            trainCars.Add(car);
        }
    }

    private void MoveHead()
    {
        // Calculate distance to the target transform
        float dist = Vector3.Distance(transform.position, targetTransform.position);

        // Stop if we are close enough
        if (dist <= stoppingDistance)
        {
            return;
        }

        // Move towards the target's current position
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);

        // Rotate head to face target
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        // Record the position history for the cars to follow
        if (positionHistory.Count == 0 || Vector3.Distance(positionHistory[0], transform.position) > 0.05f)
        {
            positionHistory.Insert(0, transform.position);
        }
    }

    private void UpdateBodyPositions()
    {
        for (int i = 0; i < trainCars.Count; i++)
        {
            // Calculate exact distance required for this car
            float targetDist = (i + 1) * spacing;

            // Get position on the history line
            Vector3 setPosition = GetPointOnPath(targetDist);

            trainCars[i].transform.position = setPosition;

            // Look at the segment ahead
            Transform targetToLookAt = (i == 0) ? transform : trainCars[i - 1].transform;
            Vector3 lookDir = targetToLookAt.position - trainCars[i].transform.position;

            if (lookDir != Vector3.zero)
            {
                trainCars[i].transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }

    private Vector3 GetPointOnPath(float targetDistance)
    {
        float traversedDistance = 0f;

        // Iterate through history points to find where this car should sit
        for (int i = 0; i < positionHistory.Count - 1; i++)
        {
            Vector3 p1 = positionHistory[i];
            Vector3 p2 = positionHistory[i + 1];

            float segmentDist = Vector3.Distance(p1, p2);

            if (traversedDistance + segmentDist >= targetDistance)
            {
                float remainingDist = targetDistance - traversedDistance;
                float ratio = remainingDist / segmentDist;
                return Vector3.Lerp(p1, p2, ratio);
            }

            traversedDistance += segmentDist;
        }

        return positionHistory[positionHistory.Count - 1];
    }

    private void TrimHistory()
    {
        float maxDistNeeded = (numberOfCars + 1) * spacing;
        int estimatedPointsNeeded = Mathf.CeilToInt(maxDistNeeded / 0.05f) + 5;

        if (positionHistory.Count > estimatedPointsNeeded)
        {
            positionHistory.RemoveRange(estimatedPointsNeeded, positionHistory.Count - estimatedPointsNeeded);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the "Tracks" (history)
        if (positionHistory.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < positionHistory.Count - 1; i++)
            {
                Gizmos.DrawLine(positionHistory[i], positionHistory[i + 1]);
            }
        }

        // Visualize the connection to the target
        if (targetTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetTransform.position);
        }
    }
}