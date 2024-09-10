using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.SceneView;

public class Waypoints : MonoBehaviour
{
    /// <summary>
    /// Initialize variables and refs to create waypoints
    /// </summary>

    [Header("GameObjects")]
    public GameObject waypointPrefab; // Get the waypoint prefab

    [Header("Script Reference")]
    public CameraWaypointMove CameraWaypointMoveRef; // Get script to CameraWaypoints 

    [Header("Array")]
    public Transform[] waypointPositions; // Array to waypoints transform

    [Header("Booleans to debug and control")]
    [SerializeField] public bool CreateWaypointActivate; // Boolean to show and control when activate createwaypoint mode


    void Start()
    {
        //Initialize creation mode to false
        CreateWaypointActivate = false;

        //DEPRECIATE
        //CreateWaypoints();
    }

    /// <summary>
    /// Control the logic to generate new waypoints and activate creation mode
    /// </summary>
    void Update()
    {
        // Update creation mode to true
        if (CreateWaypointActivate == true)
        {
            // Get input from mouse
            if (Input.GetMouseButtonDown(0)) 
            {
                // Generate a raycast from the main camera to world with the left click
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // When raycast hit with the world generate a new waypoint
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    //Generate a new waypoint
                    GameObject newWaypoint = Instantiate(waypointPrefab, hitInfo.point, Quaternion.identity);
                    //Add the gaypoint generated to list of waypoints
                    CameraWaypointMoveRef.AddWaypoint(newWaypoint.transform);
                }
            }
        } 
    }

    //DEPRECIATED
    /*void CreateWaypoints()
    {
        foreach (Transform position in waypointPositions)
        {
            Instantiate(waypointPrefab, position.position, Quaternion.identity);
        }
    }*/

    //Control when creation mode is activated or desactivated
    public void ActivateCreationMode()
    {
        if (CreateWaypointActivate == true)
            CreateWaypointActivate = false;
        else
            CreateWaypointActivate = true;
    }
}
