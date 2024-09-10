using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

//Method to check waypoint instantiate
public class Instantiation_Waypoints_Test
{
    private Waypoints waypointsScript;
    private GameObject instantiatedWaypoint;

    [SetUp]
    public void SetUp()
    {
        //Load scene "AlphaShowMode"
        SceneManager.LoadScene("AlphaShowMode");
    }

    [UnityTest]
    public IEnumerator Test_WaypointInstantiation()
    {
        //Wait to load scene
        yield return new WaitForSeconds(0.5f);

        //Get waypointmanager
        GameObject waypointManager = GameObject.Find("WaypointsManager");
        Assert.IsNotNull(waypointManager, "No se encontró el 'WaypointsManager' en la escena.");
        Debug.Log("Se encontró el WaypointsManager");

        //Get waypoint script
        waypointsScript = waypointManager.GetComponent<Waypoints>();
        Assert.IsNotNull(waypointsScript, "No se encontró el script 'Waypoints' en el 'WaypointsManager'.");
        Debug.Log("Se encontro el script de waypoints");
        waypointsScript.CreateWaypointActivate = true;

        //Simulate click on position (0.2, 0.5, 9)
        Vector3 clickPosition = new Vector3(0.2f, 0.5f, 9f);
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            //Calculate position to spawn waypoint
            Vector3 spawnPosition = ray.GetPoint(distance);
            GameObject waypointPrefab = waypointsScript.waypointPrefab;
            instantiatedWaypoint = Object.Instantiate(waypointPrefab, spawnPosition, Quaternion.identity);
            Assert.IsNotNull(instantiatedWaypoint, "El waypoint no se instanció correctamente.");
            Debug.Log("Waypoint instanciado correctamente en la posición: " + spawnPosition);
        }

        //Wait to observe object
        yield return new WaitForSeconds(5f);
    }

}