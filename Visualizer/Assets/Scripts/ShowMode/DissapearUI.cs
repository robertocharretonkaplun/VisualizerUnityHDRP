using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is to turn off and turn on the UI 
/// </summary>
public class DissapearUI : MonoBehaviour
{

    /// <summary>
    /// We initialize the button array to save any buttons that we nedd dissapear
    /// The boolean is only to show us the state of the buttons
    /// </summary>

    //public Button[] Buttons;
    [SerializeField] private bool ButtonsActive;
    private GameObject[] uiObjects;

    /// <summary>
    /// Initialize the boolean true because when we load scene the UI is turn on
    /// </summary>
    void Start()
    {
        ButtonsActive = true;
        int layer = LayerMask.NameToLayer("UI");
        uiObjects = FindObjectsOfType<GameObject>();
        uiObjects = System.Array.FindAll(uiObjects, obj => obj.layer == layer);
    }

    
    /// <summary>
    /// Call void to turn on/off UI
    /// </summary>
    void Update()
    {
        UiOff_On();
        //UiOff_On(); 
    }

    /// <summary>
    /// This void control the state changes from UI with the escape button, this button is the switch
    /// </summary>
    public void UiOff_On()
    {
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
        }*/
        if (ButtonsActive == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                foreach (GameObject obj in uiObjects)
                {
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
                    obj.SetActive(true);
                }
                ButtonsActive = true;
            }
        }

    }
   

}
