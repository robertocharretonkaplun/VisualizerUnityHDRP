using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissapearUI : MonoBehaviour
{
    public Button[] ButtonsToOff;
    public Button[] ButtonsToOn;
    public Button ButtonInjteractUIOn;
    public Button ButtonInjteractUIOff;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void UIOff()
    {
        foreach (Button button in ButtonsToOff)
        {
            button.gameObject.SetActive(false);
        }
        ButtonInjteractUIOn.gameObject.SetActive(false);
        ButtonInjteractUIOff.gameObject.SetActive(true);
    }

    public void UIOn()
    {
        foreach (Button button in ButtonsToOn)
        {
            button.gameObject.SetActive(true);
        }
        ButtonInjteractUIOn.gameObject.SetActive(true);
        ButtonInjteractUIOff.gameObject.SetActive(false);
    }
}
