using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

//Class to verify correct change between scenes with a click on button
public class Charge_Scene_Test
{
    [SetUp]
    public void SetUp()
    {
        //Charge menu scene at start
        Debug.Log("Cargando Menu");
        SceneManager.LoadScene("Menu");
    }

    //Method to wait to scene is completely charge
    private IEnumerator WaitForSceneToLoad(string sceneName)
    {
        Debug.Log("Esperando a que se cargue la escena: " + sceneName);
        while (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }
        Debug.Log("Escena " + sceneName + " cargada");

        yield return null;
    }

    //Method to show all game objects in the scene
    private void ListAllGameObjects()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Debug.Log("Objeto en la escena: " + obj.name);
        }
    }

    //Method from button action
    private IEnumerator TestUIButton(string buttonName, string expectedSceneName)
    {
        Debug.Log("Buscando el botón " + buttonName );
        GameObject button = GameObject.Find(buttonName);
        Assert.IsNotNull(button, "Boton " + buttonName +" no encontrado");
        Debug.Log("Botón " + buttonName + " encontrado.");

        SceneManagerV sceneManager = button.GetComponent<SceneManagerV>();
        Assert.IsNotNull(sceneManager, "SceneManagerV no encontrado en el boton " + buttonName);
        Debug.Log("Componente SceneManagerV encontrado en el botón " + buttonName);

        //Assign scene to button
        sceneManager.gameSceneName = expectedSceneName;
        Debug.Log("Escena " + expectedSceneName + " asignada a el botón " + buttonName);

        //Simulate button click
        Debug.Log("Simulando clic en el botón " + buttonName);
        sceneManager.OnPlayButtonClick();

        //Wait that scene is completely
        yield return WaitForSceneToLoad(expectedSceneName);

        //Verify that is correct scene
        Debug.Log("Verificando que la escena cargada sea " + expectedSceneName);
        Assert.AreEqual(expectedSceneName, SceneManager.GetActiveScene().name);
        Debug.Log("La escena cargada es " + expectedSceneName + " Prueba exitosa para el botón " + buttonName);

        //Back to menu to other test
        SceneManager.LoadScene("Menu");
        yield return WaitForSceneToLoad("Menu");
    }

    //Unit test to buttons
    [UnityTest]
    public IEnumerator TestAllMenuButtons()
    {
        //Wait to scene menu is charge
        yield return WaitForSceneToLoad("Menu");

        //list all game objects in the scene
        Debug.Log("Lista de todos los objetos en la escena:");
        ListAllGameObjects();

        //test every button
        yield return TestUIButton("Desing Mode", "Alpha");
        yield return TestUIButton("Show Mode", "AlphaShowMode");
        yield return TestUIButton("Credits (1)", "Credits");
        yield return TestUIButton("Import Mode", "Import Mode");
    }
}
