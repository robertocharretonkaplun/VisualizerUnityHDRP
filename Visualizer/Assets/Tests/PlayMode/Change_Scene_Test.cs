using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

//This class is to verify the method to change scene with button
public class ChangeSceneTest
{
    //Initialize the variables to use
    private GameObject buttonObject;
    private Button button;
    private SceneManagerV sceneManager;
    private bool sceneLoaded;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        //Create the button as game object
        buttonObject = new GameObject("Button");
        button = buttonObject.AddComponent<Button>();

        //Create the Scene manager as a game object
        var sceneManagerObject = new GameObject("SceneManager");
        sceneManager = sceneManagerObject.AddComponent<SceneManagerV>();

        //Assign the scene name to change
        sceneManager.gameSceneName = "Menu";

        //Asignar el evento OnClick del botón
        button.onClick.AddListener(sceneManager.OnPlayButtonClick);

        //Call to function to change de boolean "sceneLoaded"
        SceneManager.sceneLoaded += OnSceneLoaded;
        sceneLoaded = false;

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneTransition_OnButtonClick_LoadsNewScene()
    {
        //Simulate the button click
        button.onClick.Invoke();

        //Wait to charge scene
        yield return new WaitUntil(() => sceneLoaded);

        //Verify to the scene objective is loaded
        Assert.AreEqual(sceneManager.gameSceneName, SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneManager.gameSceneName)
        {
            sceneLoaded = true;
        }
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        //Clear the created objects in the test
        if (buttonObject != null)
        {
            Object.Destroy(buttonObject);
        }

        if (sceneManager != null && sceneManager.gameObject != null)
        {
            Object.Destroy(sceneManager.gameObject);
        }

        // Change the boolean status again
        SceneManager.sceneLoaded -= OnSceneLoaded;

        yield return null;
    }
}
