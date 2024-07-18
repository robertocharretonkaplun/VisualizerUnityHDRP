using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.SceneView;

public class Waypoints : MonoBehaviour
{
    public GameObject waypointPrefab; // Referencia al prefab del waypoint
    public Transform[] waypointPositions; // Array de posiciones donde se crearán los waypoints
    [SerializeField] private bool CreateWaypointActivate;
    public CameraWaypointMove CameraWaypointMoveRef;

    void Start()
    {
        CreateWaypointActivate = false;
        //CreateWaypoints();
    }
    void Update()
    {
        if (CreateWaypointActivate == true)
        {
            if (Input.GetMouseButtonDown(0)) // Clic izquierdo del ratón
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    GameObject newWaypoint = Instantiate(waypointPrefab, hitInfo.point, Quaternion.identity);
                    CameraWaypointMoveRef.AddWaypoint(newWaypoint.transform);
                }
            }
        } 
    }

    /*void CreateWaypoints()
    {
        foreach (Transform position in waypointPositions)
        {
            Instantiate(waypointPrefab, position.position, Quaternion.identity);
        }
    }*/

    public void ActivateCreationMode()
    {
        if (CreateWaypointActivate == true)
            CreateWaypointActivate = false;
        else
            CreateWaypointActivate = true;
    }
}
