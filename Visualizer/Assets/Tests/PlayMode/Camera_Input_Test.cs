using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

//Class to check camera movements
public class CameraMovementTest
{
    private GameObject testObject;
    private CameraM cameraScript;
    private GameObject controller;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        //Load scene "Alpha"
        SceneManager.LoadScene("Alpha");
        yield return new WaitForSeconds(1f); 

        //Look main camera and get cameraM script
        var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Assert.IsNotNull(mainCamera, "No se encontró la cámara principal");
        cameraScript = mainCamera.GetComponent<CameraM>();
        Assert.IsNotNull(cameraScript, "No se encontró el script CameraM en la cámara principal");

        //Create a object to snap
        testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testObject.transform.position = new Vector3(0, 0, 5);
        yield return null;
    }

    //Method to simulate camera movement (Zoom, movement, rotation and snap)
    [UnityTest]
    public IEnumerator CameraMovementAndSnapTest()
    {

        //Simulate movement
        yield return SimulateMov(1f); 
        yield return new WaitForSeconds(1f); 

        //Simulate zoom
        yield return SimulateZoom(5f, -5f); 
        yield return new WaitForSeconds(1f); 

        //Simulate rotation
        yield return SimulateRotation(30f, 15f); 
        yield return new WaitForSeconds(1f); 

        //Simulate snap to object
        cameraScript.SetTargetObject(testObject.transform);
        yield return new WaitForSeconds(1f); 

        //Check final camera position
        Assert.IsTrue(Vector3.Distance(cameraScript.transform.position, testObject.transform.position) <= cameraScript.focusDistance + 0.1f, "La cámara no se movió al objeto correctamente.");
    }

    private IEnumerator SimulateZoom(float zoomIn, float zoomOut)
    {
        //Simulate zoom in
        for (int i = 0; i < 10; i++)
        {
            cameraScript.transform.Translate(Vector3.forward * zoomIn * cameraScript.zoomSpeed * Time.deltaTime, Space.Self);
            yield return new WaitForSeconds(0.1f);
        }

        //Simulate zoom out
        for (int i = 0; i < 10; i++)
        {
            cameraScript.transform.Translate(Vector3.back * zoomOut * cameraScript.zoomSpeed * Time.deltaTime, Space.Self);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator SimulateRotation(float rotationX, float rotationY)
    {
        //Simulate camera rotation
        for (int i = 0; i < 10; i++)
        {
            cameraScript.transform.Rotate(Vector3.up, rotationX * cameraScript.rotationSpeed * Time.deltaTime, Space.World);
            cameraScript.transform.Rotate(Vector3.right, -rotationY * cameraScript.rotationSpeed * Time.deltaTime, Space.Self);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator SimulateMov(float distance)
    {
        //Simulate camera movement
        for (int i = 0; i < 10; i++)
        {
            cameraScript.transform.Translate(Vector3.right * distance * cameraScript.panSpeed * Time.deltaTime, Space.Self);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
