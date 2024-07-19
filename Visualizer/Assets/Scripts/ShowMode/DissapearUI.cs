using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is to turn off and turn on the UI 
/// </summary>
public class DissapearUI : MonoBehaviour
{
    //DEPRECIATE
    // We initialize the button array to save any buttons that we nedd dissapear
    //public Button[] Buttons;

    /// <summary>
    /// Create a array to ui objects to save all gameobjects in layer UI
    /// The boolean is only to show us the state of the buttons
    /// </summary>

    [Header("GameObjects")]
    private GameObject[] uiObjects;

    [Header("Booleans to debug and control")]
    [SerializeField] private bool ButtonsActive; //Boolean to control when Ui is off and on
    

    /// <summary>
    /// Initialize the boolean true because when we load scene the UI is turn on
    /// Call all gameobjects in layer UI and save in uiObjects
    /// </summary>
    void Start()
    {
        ButtonsActive = true; //Initialize UI On
        int layer = LayerMask.NameToLayer("UI"); //Get UI Layer
        uiObjects = FindObjectsOfType<GameObject>(); //Add game objects to array
        uiObjects = System.Array.FindAll(uiObjects, obj => obj.layer == layer); //Unique game objects with UI Layer
    }

    
    /// <summary>
    /// Call void to turn on/off UI
    /// </summary>
    void Update()
    {
        UiOff_On(); 
    }

    /// <summary>
    /// This void control the state changes from UI with the escape button, this button is the switch
    /// </summary>
    public void UiOff_On()
    {
        //DEPRECIATE
        /*if (ButtonsActive == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                foreach (Button button in Buttons)
                {
                    button.gameObject.SetActive(false);
                }
                ButtonsActive = false;
            }
            
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                foreach (Button button in Buttons)
                {
                    button.gameObject.SetActive(true);
                }
                ButtonsActive = true;
            }
        }
        --------------------------------------------------------------------------------*/

        if (ButtonsActive == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                foreach (GameObject obj in uiObjects)
                {
                    //UI Desactivate
                    obj.SetActive(false);
                }
                ButtonsActive = false;
            }
            
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                foreach (GameObject obj in uiObjects)
                {
                    //UI Activate
                    obj.SetActive(true);
                }
                ButtonsActive = true;
            }
        }

    }
   

}
