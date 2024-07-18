using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWaypointMove : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>(); // Lista de waypoints
    public float moveSpeed = 5f; // Velocidad de movimiento
    public float waitTime = 1f; // Tiempo de espera en cada waypoint

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private bool shouldMove = false;

    void Update()
    {
        if (shouldMove && !isMoving && waypoints.Count > 0)
        {
            StartCoroutine(MoveToWaypoint());
        }
    }

    IEnumerator MoveToWaypoint()
    {
        isMoving = true;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 startPosition = transform.position;
        Vector3 endPosition = targetWaypoint.position;
        float distance = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, endPosition) > 0.1f)
        {
            float elapsedTime = Time.time - startTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime * moveSpeed / distance);
            yield return null;
        }

        transform.position = endPosition;

        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        isMoving = false;
    }

    public void AddWaypoint(Transform waypoint)
    {
        waypoints.Add(waypoint);
    }

    public void StartMoving()
    {
        shouldMove = true;
    }
}
