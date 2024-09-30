using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

//Class to check funtion to UI show mode buttons
public class UI_Show_Mode_Test
{
    private GameObject showModeCanvas;
    private GameObject Waypointmanager;
    private GameObject DissapearUI;
    private Button playButton;
    private Button pauseButton;
    private Button stopButton;
    private Button waypointsButton;
    private DissapearUI disappearUIScript;
    private ShowModeController showModeControllerScript;
    private Waypoints waypointsScript;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        //Charge scene "AlphaShowMode"
        SceneManager.LoadScene("AlphaShowMode");
        yield return new WaitForSeconds(1); 

        //Find GameObjects
        showModeCanvas = GameObject.Find("ShowModeCanvas");
        Assert.IsNotNull(showModeCanvas, "No se encontró el ShowModeCanvas en la escena.");
        Waypointmanager = GameObject.Find("WaypointsManager");
        Assert.IsNotNull(Waypointmanager, "No se encontró el ShowModeCanvas en la escena.");
        DissapearUI = GameObject.Find("DissapearUI");
        Debug.Log("Se encontro el Canvas, DissapearUI y WaypointsManager");

        //Find buttons
        playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
        stopButton = GameObject.Find("StopButton").GetComponent<Button>();
        waypointsButton = GameObject.Find("WaypointsButton").GetComponent<Button>();
       

        Assert.IsNotNull(playButton, "No se encontró el botón 'PlayButton' en la escena");
        Assert.IsNotNull(pauseButton, "No se encontró el botón 'PauseButton' en la escena");
        Assert.IsNotNull(stopButton, "No se encontró el botón 'StopButton' en la escena");
        Assert.IsNotNull(waypointsButton, "No se encontró el botón 'WaypointsButton' en la escena");
        Debug.Log("Se encontraron los botones");

        //Find scripts
        disappearUIScript = DissapearUI.GetComponent<DissapearUI>();
        Debug.Log("Se encontro DissapearUI");
        showModeControllerScript = showModeCanvas.GetComponent<ShowModeController>();
        Debug.Log("Se encontro ShowModeController");
        waypointsScript = Waypointmanager.GetComponent<Waypoints>();
        Debug.Log("Se encontro Waypoints");

        Assert.IsNotNull(disappearUIScript, "No se encontró el script 'DissapearUI' en el objeto 'ShowModeCanvas'");
        Assert.IsNotNull(showModeControllerScript, "No se encontró el script 'ShowModeController' en el objeto 'ShowModeCanvas'");
        Assert.IsNotNull(waypointsScript, "No se encontró el script 'Waypoints' en el objeto 'WaypointsManager'");
        Debug.Log("Se encontraron los scripts");
    }

    //Method to check all UI buttons and method to dissapear UI
    [UnityTest]
    public IEnumerator TestButtonFunctionality()
    {
        //Simulate play
        playButton.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(1f, Time.timeScale, "La aplicacion no está en modo de reproducción como se esperaba");
        Debug.Log("Aplicacion en play");

        //Simulate stop
        stopButton.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Aplicacion en stop");

        //Simulate play again
        playButton.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(1f, Time.timeScale, "La aplicacion no se ha reanudado correctamente después del stop");
        Debug.Log("Aplicacion en play");

        //Activate creation waypoints mode
        waypointsButton.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Se activo el modo creacion de waypoints");

        //Deactivate creation waypoints mode
        waypointsButton.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Se desactivo el modo creacion de waypoints");

        //Simulate pause
        pauseButton.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(0f, Time.timeScale, "La aplicacion no está en pausa como se esperaba");
        Debug.Log("Aplicacion en pausa");

        //Dissapear UI
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Wait");
        Assert.IsTrue(disappearUIScript.gameObject.activeSelf, "La UI debería estar activa antes de intentar desactivarla");
        Debug.Log("Esta activo el script");
        disappearUIScript.UiOff_On();
        Debug.Log("Se desactivo la UI");
        yield return new WaitForSeconds(1.0f);
        Assert.IsFalse(disappearUIScript.gameObject.activeSelf, "La UI no se desactivó al llamar a la funcion");

    }
}
