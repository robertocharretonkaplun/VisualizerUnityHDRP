using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

//Class to check camera waypoint move
public class Camera_Waypoint_Movement_Test 
{
    //Reference objects
    private Waypoints waypointsScript;
    private CameraWaypointMove cameraWaypointMoveScript;
    private ShowModeController showModeController;
    private GameObject instantiatedWaypoint;

    [SetUp]
    public void SetUp()
    {
        // Load Scene "AlphaShowMode"
        SceneManager.LoadScene("AlphaShowMode");
    }

    //Method to instance a waypoint and camera moves to waypoint
    [UnityTest]
    public IEnumerator Test_CameraMovesToWaypoint()
    {
        //Wait to load scene
        yield return new WaitForSeconds(3f);

        //Get waypoint and waypoint camera script
        GameObject waypointManager = GameObject.Find("WaypointsManager");
        Assert.IsNotNull(waypointManager, "No se encontró el 'WaypointsManager' en la escena");
        Debug.Log("Se encontró el WaypointsManager");

        waypointsScript = waypointManager.GetComponent<Waypoints>();
        Assert.IsNotNull(waypointsScript, "No se encontró el script 'Waypoints' en el 'WaypointsManager'");

        GameObject mainCamera = GameObject.Find("Main Camera");
        Assert.IsNotNull(mainCamera, "No se encontró la cámara principal en la escena");
        Debug.Log("Se encontró la Main Camera");

        cameraWaypointMoveScript = mainCamera.GetComponent<CameraWaypointMove>();
        Assert.IsNotNull(cameraWaypointMoveScript, "No se encontró el script 'CameraWaypointMove' en la cámara");
        Debug.Log("Se encontró el camera waypoint move");

        //Look show mode canvas
        GameObject showModeCanvas = GameObject.Find("ShowModeCanvas");
        Assert.IsNotNull(showModeCanvas, "No se encontró el 'ShowModeCanvas' en la escena");
        Debug.Log("Se encontró el showmodecanvas");

        //Get show mode controller
        showModeController = showModeCanvas.GetComponent<ShowModeController>();
        Assert.IsNotNull(showModeController, "No se encontró el script 'ShowModeController' en el 'ShowModeCanvas'");
        Debug.Log("Se encontró el showmodecontroller");

        //Active creation mode
        waypointsScript.CreateWaypointActivate = true;
        Assert.IsTrue(waypointsScript.CreateWaypointActivate, "El modo de creación de waypoints no se activó correctamente");
        Debug.Log("Se activo el modo de creacion de waypoints");

        //Simulate click on position (0.2, 0.5, 9)
        Vector3 clickPosition = new Vector3(0.2f, 0.5f, 9f);
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            //Calculate spawn position waypoint
            Vector3 spawnPosition = ray.GetPoint(distance);

            //Instance waypoint
            GameObject waypointPrefab = waypointsScript.waypointPrefab;
            instantiatedWaypoint = Object.Instantiate(waypointPrefab, spawnPosition, Quaternion.identity);

            //Add waypoint to list of waypoints
            cameraWaypointMoveScript.AddWaypoint(instantiatedWaypoint.transform);
            Assert.AreEqual(1, cameraWaypointMoveScript.waypoints.Count, "El waypoint no se añadió a la lista correctamente");

            Debug.Log("Waypoint instanciado y añadido a la lista correctamente en la posición: " + spawnPosition);
        }

        //Simulate play button activation
        showModeController.Play();

        //Check camera start move to waypoint
        Assert.IsTrue(cameraWaypointMoveScript.shouldMove, "La cámara no comenzó a moverse");
        Debug.Log("La cámara comenzó a moverse hacia el waypoint.");

        //Wait to camera move to waypoint
        yield return new WaitForSeconds(5f);

        //Check if camera is on the waypoint
        Transform lastWaypoint = cameraWaypointMoveScript.waypoints[0];
        Assert.AreEqual(lastWaypoint.position, cameraWaypointMoveScript.transform.position, "La cámara no llegó al waypoint esperado");
        Debug.Log("La cámara llegó al waypoint");
    }

    [TearDown]
    public void TearDown()
    {
        //Clear scene
        if (instantiatedWaypoint != null)
        {
            GameObject.Destroy(instantiatedWaypoint);
        }
    }
}
