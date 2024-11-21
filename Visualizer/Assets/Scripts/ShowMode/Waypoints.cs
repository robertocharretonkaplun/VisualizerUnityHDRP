using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.SceneView;

[RequireComponent(typeof(LineRenderer))]
public class Waypoints : MonoBehaviour
{
    /// <summary>
    /// Initialize variables and refs to create waypoints
    /// </summary>

    [Header("GameObjects")]
    public GameObject waypointPrefab; //Get the waypoint prefab

    [Header("Script Reference")]
    public CameraWaypointMove CameraWaypointMoveRef; //Get script to CameraWaypoints 
    public CharacterWaypointMove CharacterWaypointMoveRef;

    [Header("Array")]
    public Transform[] waypointPositions; //Array to waypoints transform

    [Header("Booleans to debug and control")]
    [SerializeField] private bool CreateWaypointActivate; //Boolean to show and control when activate createwaypoint mode

    [Header("Line Renderer")]
    private LineRenderer lineRenderer; //LineRenderer component to draw lines between waypoints
    public Material lineMaterial; //Material with texture for LineRenderer

    [Header("Interpolation Settings")]
    public int interpolationPointsPerSegment = 10; //Interpolation points per segment to smooth the curve

    void Start()
    {
        //Initialize creation mode to false
        CreateWaypointActivate = false;

        //Get the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

        //Assign the material to LineRenderer to use a texture instead of a solid color
        lineRenderer.material = lineMaterial;

        //Initialize the number of positions in the LineRenderer
        lineRenderer.positionCount = 0;

        //Adjust texture mode to repeat instead of stretch
        lineRenderer.textureMode = LineTextureMode.Tile;

        //Set the line thickness
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

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
                    CharacterWaypointMoveRef.AddWaypoint(newWaypoint.transform);
                    // Update the LineRenderer to reflect the new waypoints
                    UpdateLineRenderer();
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

    /// <summary>
    /// Updates the LineRenderer to draw lines between waypoints
    /// </summary>
    //Version whitout curves
    /*public void UpdateLineRenderer()
    {
        //Clean the waypoint list by removing null references
        //CleanWaypointList();

        //Set the number of positions in the LineRenderer based on the number of waypoints
        lineRenderer.positionCount = CharacterWaypointMoveRef.waypoints.Count;

        //Draw lines between each waypoint
        for (int i = 0; i < CharacterWaypointMoveRef.waypoints.Count; i++)
        {
            //Set the line position to match the corresponding waypoint
            lineRenderer.SetPosition(i, CharacterWaypointMoveRef.waypoints[i].position);
        }

        //Adjust texture tiling based on line length
        float textureTiling = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1)) * 0.1f;
        lineRenderer.material.mainTextureScale = new Vector2(textureTiling, 1);
    }*

    //Removes null references from the waypoint list
    /*public void CleanWaypointList()
    {
        CharacterWaypointMoveRef.waypoints.RemoveAll(item => item == null);
    }*/

    //Updates the LineRenderer with interpolation for smooth curves
    public void UpdateLineRenderer()
    {
        // Generates a list of interpolated points along the curve
        List<Vector3> interpolatedPoints = GenerateSmoothCurve(CharacterWaypointMoveRef.waypoints);

        // Sets the number of positions in the LineRenderer equal to the count of interpolated points
        lineRenderer.positionCount = interpolatedPoints.Count;

        // Sets each interpolated point as a position in the LineRenderer to draw the curve
        for (int i = 0; i < interpolatedPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, interpolatedPoints[i]);
        }

        //Adjust texture tiling based on line length
        float textureTiling = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1)) * 0.1f;
        lineRenderer.material.mainTextureScale = new Vector2(textureTiling, 1);
    }

    /// <summary>
    /// Generates a smooth curve using Catmull-Rom interpolation between waypoints
    /// </summary>
    private List<Vector3> GenerateSmoothCurve(List<Transform> waypoints)
    {
        //List to store the smooth curve points
        List<Vector3> curvePoints = new List<Vector3>();

        //If there are fewer than two waypoints, a curve can't be generated, so return the empty list
        if (waypoints.Count < 2) return curvePoints;

        //Loops through all waypoints except the last to generate curve segments
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            //Defines control points for Catmull-Rom calculation: P0, P1, P2, and P3
            Vector3 p0 = (i == 0) ? waypoints[i].position : waypoints[i - 1].position;
            Vector3 p1 = waypoints[i].position;
            Vector3 p2 = waypoints[i + 1].position;
            Vector3 p3 = (i == waypoints.Count - 2) ? waypoints[i + 1].position : waypoints[i + 2].position;

            //Generates intermediate points between P1 and P2 according to the interpolation factor for a smoother curve
            for (int j = 0; j <= interpolationPointsPerSegment; j++)
            {
                float t = j / (float)interpolationPointsPerSegment; //Interpolation progress from 0 to 1
                Vector3 interpolatedPoint = CatmullRomInterpolation(p0, p1, p2, p3, t); //Interpolated point using Catmull-Rom
                curvePoints.Add(interpolatedPoint); //Adds the point to the list
            }
        }

        //Returns the list of interpolated points that define the curve
        return curvePoints;
    }

    /// <summary>
    /// Performs Catmull-Rom interpolation between four control points
    /// </summary>
    /// <param name="p0">Punto 0 (antes del segmento)</param>
    /// <param name="p1">Punto 1 (inicio del segmento)</param>
    /// <param name="p2">Punto 2 (fin del segmento)</param>
    /// <param name="p3">Punto 3 (después del segmento)</param>
    /// <param name="t">Factor de interpolación (0 a 1)</param>
    /// <returns>Punto interpolado</returns>
    private Vector3 CatmullRomInterpolation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        //Calculates the square and cube of t, representing the interpolation progress
        float t2 = t * t; // t²
        float t3 = t2 * t; // t³

        //Applies the Catmull-Rom interpolation formula, adding each term with its factors
        return 0.5f * (
            (2f * p1) +                 // Termino base, escalado con el punto inicial (p1)
            (-p0 + p2) * t +            // Primer término lineal entre el punto anterior (p0) y el siguiente (p2)
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 + // Término cuadrático que suaviza la transición
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3); // Término cúbico para un ajuste final en la curva
    }
}
