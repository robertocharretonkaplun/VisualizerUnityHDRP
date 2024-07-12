using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissapearUI : MonoBehaviour
{
    public Button[] Buttons;
    [SerializeField] private bool ButtonsActive;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        ButtonsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        UiOff_On(); 
    }

    private void FixedUpdate()
    {
        if(Time.timeScale > 0f) 
        {
            Debug.Log("Animacion");
        }
        else if(Time.timeScale < 1f) 
        {
            Debug.Log("Pausa");
        }
        
    }

    public void Play()
    {
        Time.timeScale = 1.0f;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void UiOff_On()
    {
        if (ButtonsActive == true)
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
        
    }

    public void StopButton()
    {
       
    }
}
