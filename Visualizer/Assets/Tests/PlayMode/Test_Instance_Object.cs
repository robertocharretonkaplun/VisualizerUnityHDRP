using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

//Method to check the correctly instance object
public class Test_Instance_Object
{
    private LevelEditor_Manager levelEditor_Manager;
    private GameObject instantiatedObject;

    [SetUp]
    public void SetUp()
    {
        //Load Alpha scene
        SceneManager.LoadScene("Alpha");
    }

    [UnityTest]
    public IEnumerator Test_ItemInstantiation()
    {
        //Wait to scene is loaded
        yield return new WaitForSeconds(1f);

        //Get manager 
        levelEditor_Manager = LevelEditor_Manager.Instance;
        Assert.IsNotNull(levelEditor_Manager, "El LevelEditor_Manager no está inicializado.");
        Debug.Log("Fue encontrado el level editor manager");

        //Simulate click on screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(1, 1, 1));
        //make a plane to collision with raycast
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        if (plane.Raycast(ray, out float distance))
        {
            //cast ray
            Vector3 spawnPosition = ray.GetPoint(distance);
            //instance item
            levelEditor_Manager.SpawnItemAtPosition(2, spawnPosition);
        }

        //Verify instance object
        instantiatedObject = GameObject.FindObjectOfType(levelEditor_Manager.ItemPrefabs[0].GetType()) as GameObject;
        Assert.IsNotNull(instantiatedObject, "El objeto no se instanció correctamente.");

        //Wait 5 seconds to check the object instance
        yield return new WaitForSeconds(5f);
    }

    [TearDown]
    public void TearDown()
    {
        //Clear scene
        if (instantiatedObject != null)
        {
            GameObject.Destroy(instantiatedObject);
        }
    }
}
