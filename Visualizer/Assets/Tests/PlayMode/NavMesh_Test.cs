using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

//Method to verify movement from navmesh agent
public class NavMeshTest
{
    private NavMeshController navMeshController;
    private NavMeshAgent agent;
    private Camera camera;
    private GameObject targetObject;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        //Load NavMesh scene
        SceneManager.LoadScene("NavMesh");
        yield return new WaitForSeconds(1f); 

        //Find navmesh controller
        var agentObject = GameObject.FindObjectOfType<NavMeshController>();
        Assert.IsNotNull(agentObject, "No se encontró el NavMeshController en la escena");

        //Asign camera and navmesh agent
        navMeshController = agentObject.GetComponent<NavMeshController>();
        agent = navMeshController.agent;
        camera = navMeshController.cam;

        //Make object to simulate click/destination to agent
        targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetObject.transform.position = new Vector3(-5f, 0f, 2f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator AgentMovesToTargetOnClickSimulation()
    {
        //Simulate click to move agent
        yield return SimulateClickOnPosition(targetObject.transform.position);

        //wait to start agent move
        yield return new WaitForSeconds(4f);

        //Check if my agent is on correct position
        Assert.IsTrue(Vector3.Distance(agent.destination, targetObject.transform.position) < 1.0f, "El agente no se movio correctamente al lugar");
        Debug.Log("El agente se movio al lugar");
    }

    //Method to simulate click
    private IEnumerator SimulateClickOnPosition(Vector3 targetPosition)
    {
        //make a world position
        Vector3 screenPoint = camera.WorldToScreenPoint(targetPosition);

        //Simulate click on the world position
        Ray ray = camera.ScreenPointToRay(screenPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Set destination to world position
            navMeshController.agent.SetDestination(hit.point);
        }

        yield return null; 
    }
}
