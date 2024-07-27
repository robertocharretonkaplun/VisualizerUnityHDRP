using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the agent in NavMesh to be able to move through the level.
/// </summary>

public class NavMeshController : MonoBehaviour
{
    /// <summary>
    /// The main camera will help us to obtain the position of the user's click.
    /// </summary>
    public Camera cam;

    /// <summary>
    /// NavMesh Agent is the object that will move to the position where the user clicked.
    /// </summary>
    public UnityEngine.AI.NavMeshAgent agent;

    /// <summary>
    /// This method checks whether the user has clicked and, if so, calculates a destination 
    /// for the NavMesh agent based on the click position in the level.
    /// </summary>
    void Update()
    {
        // Checks if the left mouse button (button 0) has been pressed
        if (Input.GetMouseButtonDown(0))
        {
            // In this part it takes the position of the mouse on the screen and projects a ray from the camera
            //  towards that position in the world (The ray is an invisible line used to detect
            //  collisions with objects in the scene.)
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // It will store information about the collision, if any. RaycastHit is a structure that contains
            //  data about the point of impact, the distance from the origin of the beam and where the collider.
            RaycastHit hit;
            // If the beam collides with any object in the scene. Collision information is stored in the variable hit.
            // The out keyword indicates that hit will be an output parameter that will receive the value
            // calculated within the method.
            if (Physics.Raycast(ray, out hit))
            {
                // If the ray collides with an object, this method sets the target of the NavMeshAgent 
                // to the collision point (hit.point). The agent will automatically move to that position
                // using the navigation provided by Unity's NavMesh system.
                agent.SetDestination(hit.point);
            }
        }
    }
}
