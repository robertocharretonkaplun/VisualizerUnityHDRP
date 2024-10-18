using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWaypointMove : MonoBehaviour
{
    /// <summary>
    /// Initialize variables and list to control how and when start camera movement throught the waypoints
    /// </summary>

    [Header("List of waytpoints to use and variable to show which has traveled")]
    public List<Transform> waypoints = new List<Transform>();
    [SerializeField] private int currentWaypointIndex = 0;

    [Header("Variables to move camera")]
    public float moveSpeed = 5f;
    public float waitTime = 1f;

    [Header("Booleans to debug and when the camera should start moving")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool shouldMove = false;

    private Vector3 ResetStopPosition;

    /// <summary>
    /// Establish a if condition with the booleans and list of waypoints to start coroutine to move
    /// </summary>
    void Update()
    {
        CleanWaypointList();

        if (shouldMove && !isMoving && waypoints.Count > 0)
        {
            StartCoroutine(MoveToWaypoint());
        }
    }

    /// <summary>
    /// Create the IEnumerator to contain logic to move camera
    /// </summary>
    IEnumerator MoveToWaypoint()
    {
        //Establish boolean on true to control when start to move
        isMoving = true;

        //Get transform component to waypoint from the index
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        //Get actual transform position how start position
        Vector3 startPosition = transform.position;

        //Get transform position from the targetwaypoint how end position
        Vector3 endPosition = targetWaypoint.position;

        //Calculate distance beetwen start position and end position
        float distance = Vector3.Distance(startPosition, endPosition);

        //Save time when IEnumerator start
        float startTime = Time.time;

        //This while is to achieve fluid movement when transferring between waypoints
        while (Vector3.Distance(transform.position, endPosition) > 0.1f)
        {
            //Calculate time transcurrent beetwen start time and actual time
            float elapsedTime = Time.time - startTime;
            //The movement is because a generate linear interpolation to get a slow and fluid movement
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime * moveSpeed / distance);
            yield return null;
        }

        //Update end position value
        transform.position = endPosition;

        //Wait seconds to travel a new waypoint
        yield return new WaitForSeconds(waitTime);

        //Update count to index from waypoints
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;

        //Establish boolean on true to control when stop to move
        isMoving = false;
    }

    /// <summary>
    /// Void to add waypoints to list of waypoints
    /// </summary>

    public void AddWaypoint(Transform waypoint)
    {
        waypoints.Add(waypoint);
    }

    /// <summary>
    /// Void to control the start move
    /// </summary>
    public void StartMoving()
    {
        shouldMove = true;
        ResetStopPosition = transform.position;
    }

    /// <summary>
    /// Void to control the stop move
    /// </summary>
    public void StopMoving()
    {
        if (shouldMove == true)
        {
            shouldMove = false;
            StopAllCoroutines();
            currentWaypointIndex = 0;
            transform.position = ResetStopPosition;
            isMoving = false;
        }

    }

    public void CleanWaypointList()
    {
        //Removes null references from the waypoint list
        waypoints.RemoveAll(item => item == null);
        //Update lines after delete a waypoint
        FindObjectOfType<Waypoints>().UpdateLineRenderer(); 
    }


}
