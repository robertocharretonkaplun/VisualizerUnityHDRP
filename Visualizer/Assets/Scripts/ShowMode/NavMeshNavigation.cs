using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Class to navigate in runtime
public class NavMeshNavigation : MonoBehaviour
{
    //Reference to my main camera
    public Camera CameraDefault;
    //Reference to my camera inside capsule/player
    public Camera CameraFirstPerson;
    //Reference to Navmesh
    public UnityEngine.AI.NavMeshAgent agent;

    //Bool to manage change mode
    public bool NavigationMode = true;

    //Bool to set when u can move agent with click and when u cant move
    public bool MoveActivate = false;

    void Start()
    {
        //Get my Navmesh component
        agent = GetComponent<NavMeshAgent>();
        //At start my First Person camera is off
        CameraFirstPerson.enabled = false;
    }

    void Update()
    {
        //If I push F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            //I`ll change my camera 
            ChangeNavigationMode();
        }

        //If I push C key
        if (Input.GetKeyDown(KeyCode.C))
        {
            //I`ll modify if i can move my agent or not
            ChangeMoveMode();
        }

        //If I push my left click mouse
        if (Input.GetMouseButtonDown(0) && MoveActivate)
        {
            //Use active camera to make a Raycast to set agent destination
            Camera activeCamera = NavigationMode ? CameraFirstPerson : CameraDefault;
            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

    }

    //Method to change between my main camera or my First Person camera
    void ChangeNavigationMode()
    {
        NavigationMode = !NavigationMode;

        //Enable and disable my cameras
        CameraDefault.enabled = !NavigationMode;
        CameraFirstPerson.enabled = NavigationMode;
    }

    //Method to cahnge between my modes to activate move
    void ChangeMoveMode()
    {
        MoveActivate = !MoveActivate;
    }
}
