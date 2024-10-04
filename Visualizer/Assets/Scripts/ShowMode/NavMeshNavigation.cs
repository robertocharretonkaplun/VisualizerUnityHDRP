using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Class to navigate in runtime
public class NavMeshNavigation : MonoBehaviour
{

    public Camera cam;
    public UnityEngine.AI.NavMeshAgent agent;

    //Bool to manage change mode
    public bool NavigationMode;

    void Start()
    {
       agent = GetComponent<NavMeshAgent>();
       cam = GetComponent<Camera>();
       NavigationMode = false;
    }

    void Update()
    {
        
    }
}
