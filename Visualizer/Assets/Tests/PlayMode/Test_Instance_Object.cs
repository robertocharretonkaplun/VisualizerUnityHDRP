using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class ObjectInstantiationTests
{
    private LevelEditor_Manager levelEditorManager;
    private GameObject instantiatedObject;

    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("Alpha"); //Initialize your scene
    }

    //Wait to scene is completely charge
    private IEnumerator WaitForSceneToLoad(string sceneName)
    {
        while (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }
        yield return null; 
    }

    [UnityTest]
    public IEnumerator TestObjectInstantiation()
    {
        yield return WaitForSceneToLoad("Alpha");

        levelEditorManager = Object.FindObjectOfType<LevelEditor_Manager>();
        Assert.IsNotNull(levelEditorManager, "No se encontró el LevelEditor_Manager en la escena.");
        Debug.Log("Se encontro el LevelEditorManager");

        //Simulate click in the object button
        var itemController = levelEditorManager.ItemButtons[0];
        itemController.ButtonClick();

        //Simulate click on screen
        yield return SimulateClickOnScreen();
        Debug.Log("Se simulo el click");

        //Verify the object has instance
        instantiatedObject = GameObject.Find(levelEditorManager.ItemPrefabs[0].name + "(Clon)");
        Debug.Log("Objeto instanciado");
        Assert.IsNotNull(instantiatedObject, "El objeto no se ha instanciado correctamente.");
        


        //Verify position from instanced object
        Assert.IsTrue(instantiatedObject.transform.position != Vector3.zero, "La posición del objeto instanciado no es la correcta.");
    }

    //Method to simulate click on screen
    private IEnumerator SimulateClickOnScreen()
    {
        //Make a ray from the center of screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            //Create a plane to collision with ray
            Plane plane = new Plane(Vector3.up, new Vector3(0, levelEditorManager.PlaneHeight, 0));
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 spawnPosition = ray.GetPoint(distance);

                //Simulate click with the level editor manager logic
                var itemPrefab = levelEditorManager.ItemPrefabs[levelEditorManager.CurrentButtonPressed];
                Quaternion spawnRotation = itemPrefab.transform.rotation;

                spawnPosition.y = itemPrefab.transform.position.y;

                var placeItemCommand = new PlaceItemCommand(itemPrefab, spawnPosition, spawnRotation);
                
            }
        }

        yield return null;
    }

    //Clear everything
    [TearDown]
    public void TearDown()
    {
        if (instantiatedObject != null)
        {
            Object.Destroy(instantiatedObject);
        }
    }
}
