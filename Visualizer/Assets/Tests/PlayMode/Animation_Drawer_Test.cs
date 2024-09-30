using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

//Class is to check play animation when "AccesBar_ContentBtn" is clicked
public class Animation_Drawer_Test
{
    //References to script, canvas, button and drawer
    private GameObject canvas;
    private GameObject accessBarContentBtn;
    private GameObject drawer;
    private AnimationC animationCScript;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        //Charge "Alpha" scene
        SceneManager.LoadScene("Alpha");

        //Wait to scene is charge
        yield return new WaitForSeconds(1); 
        Debug.Log("Escena cargada");

        //Find canvas object and assign 
        canvas = GameObject.Find("Canvas");
        Assert.IsNotNull(canvas, "No se encontró el Canvas en la escena.");
        Debug.Log("Canvas encontrado");

        //Find button object and assign
        accessBarContentBtn = GameObject.Find("AcccessBar_ContentBtn");
        Assert.IsNotNull(accessBarContentBtn, "No se encontró el botón 'AccessBar_ContentBtn' en la escena.");
        Debug.Log("Boton encontrado");

        //Find Drawer object and assign
        drawer = GameObject.Find("Drawer");
        Assert.IsNotNull(drawer, "No se encontró el objeto 'Drawer' en la escena.");
        Debug.Log("Drawer encontrado");

        //Find Script and assign
        animationCScript = drawer.GetComponent<AnimationC>();
        Assert.IsNotNull(animationCScript, "No se encontró el script 'AnimationC' en el objeto 'Drawer'.");
        Debug.Log("Script encontrado");
    }

    //Method to check animation to drawerContentBtn whe is invoked
    [UnityTest]
    public IEnumerator TestAnimationIsPlaying()
    {
        //Simulate click on button to drawer is down
        Button button = accessBarContentBtn.GetComponent<Button>();
        button.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);

        //Check the animation is down
        Assert.IsTrue(animationCScript.isDown, "La animación se ha reproducido hacia abajo como se esperaba.");
        Assert.IsFalse(animationCScript.isUp, "La animación no ha cambiado el estado a 'isUp = false' como se esperaba.");

        //Wait seconds to check end animation
        yield return new WaitForSeconds(1f);

        //Simulate click on button to drawer is up
        button.onClick.Invoke();
        yield return new WaitForSeconds(0.1f);

        //Check the animation is up
        Assert.IsTrue(animationCScript.isUp, "La animación se ha reproducido hacia arriba como se esperaba.");
        Assert.IsFalse(animationCScript.isDown, "La animación no ha cambiado el estado a 'isDown = false' como se esperaba.");

        yield return new WaitForSeconds(1f);
    }
}
